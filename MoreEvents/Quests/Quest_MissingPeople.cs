using MapGeneratorBlueprints.MapGenerator;
using MoreEvents.Communications;
using MoreEvents.Events;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Quests
{
    public class Quest_MissingPeople : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_MissingPeople;

        public override string CardLabel => "Quest_MissingPeople_CardLabel".Translate();

        public override string Description => "Quest_MissingPeople_Description".Translate(Faction.Name, minDays, passedDays, TicksToPass.TicksToDays().ToString("f2"));

        public override string PlaceLabel => saved? "Quest_MissingPeople_PlaceLabelSaved".Translate() : "Quest_MissingPeople_PlaceLabel".Translate();

        public override string ExpandingIconPath => saved ? expandingIconPath2 : "Quests/Quest_MissingPeople";

        private string expandingIconPath2 => "Quests/Quest_MissingPeople2";

        public float FindChance => 0.35f;
        private int minDays = 5;
        private int passedDays = 8;

        private bool saved = false;

        private List<Pawn> savedPawns = new List<Pawn>();
        public List<Pawn> SavedPawns => savedPawns;

        public Quest_MissingPeople()
        {
        }

        public Quest_MissingPeople(int daysLeft, int minDays = 5, int passedDays = 8)
        {
            TicksToPass = daysLeft * 60000;
            this.minDays = minDays;
            this.passedDays = passedDays;

        }

        public override bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            EventSettings settings = Settings.EventsSettings["Quest_MissingPeople"];
            if (!settings.Active)
                return false;

            Faction = questPawn.Faction;
            Map map = questPawn.Map ?? Find.AnyPlayerHomeMap;

            if (!IncidentWorker_Quest_MissingPeople.TryGetNewTile(map.Tile, out int newTile))
                return false;

            TicksToPass = Rand.Range(5, 9) * 60000;
            minDays = Rand.Range(5, 20);
            passedDays = Rand.Range(minDays + 3, minDays + 7);
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();

            int additionalValue = passedDays * 15;
            GenerateRewards(GetQuestThingFilter(), new FloatRange(700 + additionalValue, 1400 + additionalValue), new IntRange(3, 8), null, null);

            ShowInConsole = false;

            QuestsManager.Communications.AddQuestPawn(questPawn, this);
            QuestsManager.Communications.AddQuest(this);

            return true;
        }

        public override void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            IncidentWorker_Quest_MissingPeople.TryGetNewTile(quester.Pawn.Tile, out int newTile);

            LookTargets target = new LookTargets(newTile);
            Target = target;

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = newTile;
            questPlace.SetFaction(Faction);
            questPlace.Init(this);
            questPlace.RemoveAfterLeave = false;

            Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            base.TakeQuestByQuester(quester, notify);
        }

        public override void Notify_CaravanFormed(QuestSite site, Caravan caravan)
        {
            foreach (var pawn in savedPawns)
            {
                if (pawn != null && !pawn.Dead)
                {
                    caravan.AddPawn(pawn, false);

                    pawn.DestroyOrPassToWorld();
                }
            }

            saved = true;
            ResetIcon();

            Settlement settlement = Find.WorldObjects.Settlements.Where(delegate (Settlement settl)
            {
                return settl.Faction == Faction && Find.WorldReachability.CanReach(site.Tile, settl.Tile);
            }).OrderBy(x => Find.WorldGrid.ApproxDistanceInTiles(site.Tile, x.Tile)).FirstOrDefault();
            if (settlement != null)
            {
                int arrivalTime = CaravanArrivalTimeEstimator.EstimatedTicksToArrive(site.Tile, settlement.Tile, caravan);
                TicksToPass = arrivalTime  + (3 * 60000);
                UnlimitedTime = false;
                
                Find.LetterStack.ReceiveLetter("Quest_MissingPeople_Stage2Title".Translate(), "Quest_MissingPeople_Stage2".Translate(TicksToPass.ToStringTicksToDays("0.#")), LetterDefOf.PositiveEvent);
                site.Tile = settlement.Tile;

                Target = new LookTargets(site.Tile);
            }

            Current.Game.DeinitAndRemoveMap(site.Map);
        }

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(site.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override void PostMapGenerate(Map map)
        {
            foreach(var data in MapDefOfLocal.Camp.MapData)
            {
                if(data.key.Kind != null)
                {
                    data.key.Kind = Faction.RandomPawnKind();
                }
            }

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.Camp, map, out savedPawns, fog: false, unFogRoom: true, spawnPawns: true, createRoof: true, forceFaction: Faction.OfPlayer);

            if (TryGetEnemyFaction(Faction, out Faction enemyFac))
            {
                int @int = Rand.Int;
                IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
                raidParms.forced = true;
                raidParms.faction = enemyFac;
                raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                raidParms.points = Rand.Range(400, 1000);
                raidParms.pawnGroupMakerSeed = @int;
                var incident = new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms);
                Find.Storyteller.TryFire(incident);
            }
        }

        private bool TryGetEnemyFaction(Faction hostileTo, out Faction faction)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where !x.IsPlayer && !x.def.hidden && !x.defeated && x.def.humanlikeFaction&& x.HostileTo(hostileTo)
                 select x).TryRandomElement(out faction))
            {
                return true;
            }
            return false;
        }

        public override void Tick()
        {
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            if (saved)
            {
                CaravanArrivalAction_TransferMissingPeople caravanAction2 = new CaravanArrivalAction_TransferMissingPeople(mapParent, this);
                foreach (var opt in CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction2, "Quest_CheckMissingPeople_Action2".Translate(), caravan, mapParent.Tile, mapParent))
                {
                    yield return opt;
                }
            }
            else
            {
                CaravanArrivalAction_CheckMissingPeople caravanAction = new CaravanArrivalAction_CheckMissingPeople(mapParent, this);
                foreach (var opt in CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "Quest_CheckMissingPeople_Action".Translate(), caravan, mapParent.Tile, mapParent))
                {
                    yield return opt;
                }
            }
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);
            filter.SetAllow(ThingCategoryDefOf.Manufactured, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);

            return filter;
        }

        public override void GenerateRewards(ThingFilter filter, FloatRange totalValue, IntRange countRange, TechLevel? techLevel, float? totalMass)
        {
            base.GenerateRewards(filter, totalValue, countRange, techLevel, totalMass);

            filter.SetDisallowAll();
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms2 = default;
            parms2.totalMarketValueRange = new FloatRange(300, 800);
            parms2.countRange = new IntRange(2, 4);
            parms2.filter = filter;

            maker.fixedParams = parms2;

            List<Thing> items = maker.Generate();
            items.ForEach(i => Rewards.Add(i));
        }

        public override void SiteTick()
        {
            TicksToPass--;

            if(TicksToPass <= 0)
            {
                if(saved && savedPawns.Count > 0)
                    Site.EndQuest(null, EndCondition.Timeout);
                else
                    Site.EndQuest(null, EndCondition.Fail);
            }
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            base.EndQuest(caravan, condition);

            var interaction = QuestsManager.Communications.FactionManager.GetInteraction(Faction);

            if(condition == EndCondition.Timeout)
            {
                if (savedPawns != null)
                {
                    if (saved && savedPawns.Count > 0)
                    {
                        foreach (var pawn in savedPawns)
                        {
                            if(pawn != null && pawn.Faction != Faction)
                                pawn.SetFaction(Faction);
                        }

                        Faction.TryAffectGoodwillWith(Faction.OfPlayer, -50);
                        Find.LetterStack.ReceiveLetter("Quest_MissingPeople_ThiefTitle".Translate(), "Quest_MissingPeople_Thief".Translate(), LetterDefOf.NegativeEvent);

                        for (int i = 0; i < QuestsManager.Communications.Components.Count; i++)
                        {
                            CommunicationComponent comp = QuestsManager.Communications.Components[i];
                            if (comp is ScoutingComp comp2 && comp2.Faction == Faction)
                            {
                                QuestsManager.Communications.RemoveComponent(comp);
                            }
                        }

                        CommOption_GetHelp.AddComponentWithStack(Faction, -999);
                    }

                    if (interaction != null)
                        interaction.Trust -= 5;
                }
            }
            if(condition == EndCondition.Fail)
            {
                Faction.TryAffectGoodwillWith(Faction.OfPlayer, -25);
                if (interaction != null)
                    interaction.Trust -= 15;
            }
            if(condition == EndCondition.Success)
            {
                Faction.TryAffectGoodwillWith(Faction.OfPlayer, 20);

                if(savedPawns != null)
                {
                    if (interaction != null)
                        interaction.Trust += savedPawns.Count * 5;
                }
            }
        }

        public override string GetInspectString()
        {
            return saved ? "Quest_MissingPeople_InspectString2".Translate(TicksToPass.TicksToDays().ToString("f2")) : "Quest_MissingPeople_InspectString".Translate(TicksToPass.TicksToDays().ToString("f2"));
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref savedPawns, "savedPawns", LookMode.Reference);
            Scribe_Values.Look(ref minDays, "minDays");
            Scribe_Values.Look(ref passedDays, "passedDays");
            Scribe_Values.Look(ref saved, "saved");
        }
    }
}
