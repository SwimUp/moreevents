using QuestRim;
using RimOverhaul;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Quests
{
    public class Quest_SuppressionRebellion : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => "Quest_SuppressionRebellion_CardLabel".Translate();

        public override string Description => "Quest_SuppressionRebellion_Description".Translate(Faction.Name);

        public override string ExpandingIconPath => "Quests/Quest_SuppressionRebellion";

        public override string PlaceLabel => "Quest_SuppressionRebellion_PlaceLabel".Translate();

        public override int SuccessTrustAffect => 25;

        public override int FailTrustAffect => -20;

        public override int TimeoutTrustAffect => -20;

        public Settlement RebelSettlement;

        public bool AttackFail = false;
        public bool Won = false;
        private bool supress = false;

        private int rebelFight = 15000;

        public Quest_SuppressionRebellion()
        {

        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.BuildingsArt, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);

            return filter;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AttackFail, "AttackFail");
            Scribe_Values.Look(ref Won, "Won");
            Scribe_Values.Look(ref rebelFight, "rebelFight");
            Scribe_Values.Look(ref supress, "supress");
            Scribe_References.Look(ref RebelSettlement, "RebelSettlement");
        }

        public override string GetInspectString()
        {
            return $"InspectString_Timer".Translate(UnlimitedTime ? rebelFight.TicksToDays().ToString("f2") : TicksToPass.TicksToDays().ToString("f2"));
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToRebelMap caravanAction = new CaravanArrivalAction_EnterToRebelMap(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = Mathf.Clamp(Rand.Range(300, 500) * (int)Faction.def.techLevel, 450, 1600),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack
            };

            List<Pawn> rebels = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            IntVec3 spot;
            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out spot, out IntVec3 second);
            foreach (var rebel in rebels)
            {
                if (CellFinder.TryFindRandomCellNear(spot, map, 6, (IntVec3 x) => x.Standable(map) && !x.Fogged(map), out IntVec3 loc))
                {
                    GenSpawn.Spawn(rebel, loc, map, Rot4.Random);
                    rebel.SetFaction(Faction.OfAncientsHostile);
                }
            }

            LordJob lordJob = new LordJob_AssaultColony(Faction.OfPlayer, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
            LordMaker.MakeNewLord(Faction.OfAncientsHostile, lordJob, map, rebels);

            supress = true;
            UnlimitedTime = true;

            ShowInfo();
        }

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if (HostileUtility.AnyNonDownedHostileOnMap(site.Map, Faction.OfAncientsHostile))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override void SiteTick()
        {
            base.SiteTick();

            if (AttackFail)
            {
                rebelFight--;

                if(rebelFight <= 0)
                {
                    DoRebelResult();
                }
            }
            else
            {
                if (Find.TickManager.TicksGame % 500 == 0)
                {
                    if (Site.HasMap && !Won)
                    {
                        CheckWon();
                    }
                }
            }
        }

        public override void Tick()
        {
            if (!UnlimitedTime && !AttackFail)
            {
                TicksToPass--;
                if (TicksToPass <= 0)
                {
                    AttackFail = true;
                    UnlimitedTime = true;
                    Site.Tile = RebelSettlement.Tile;
                }
            }
        }

        public void DoRebelResult()
        {
            float successChance = 0.45f;

            if(Rand.Chance(successChance))
            {
                Faction.Notify_LeaderDied();

                string playerAffect = "";
                if(supress)
                {
                    Faction.TryAffectGoodwillWith(Faction.OfPlayer, -15);
                    playerAffect = "Quest_SuppressionRebelplayerAffect".Translate();
                }
                else
                {
                    playerAffect = "Quest_SuppressionRebelNonplayerAffect".Translate();
                }

                Find.LetterStack.ReceiveLetter("Quest_SuppressionRebellionRebelWinTitle".Translate(),
                    "Quest_SuppressionRebellionRebelWinDesc".Translate(RebelSettlement.Name, playerAffect), LetterDefOf.NeutralEvent);
            }
            else
            {
                Faction faction = DoNewFaction();

                Find.LetterStack.ReceiveLetter("Quest_SuppressionRebellionNewFactionTitle".Translate(),
                    "Quest_SuppressionRebellionNewFactionDesc".Translate(RebelSettlement.Name, faction.Name), LetterDefOf.PositiveEvent);

                int tile = TileFinder.RandomSettlementTileFor(faction);
                Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                settlement.SetFaction(faction);
                settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);
                settlement.Tile = tile;

                Find.WorldObjects.Add(settlement);
            }

            Site.EndQuest(null, EndCondition.None);
        }

        private Faction DoNewFaction()
        {
            Faction faction = new Faction();
            faction.def = Faction.def;
            faction.colorFromSpectrum = FactionGenerator.NewRandomColorFromSpectrum(faction);
            faction.Name = NameGenerator.GenerateName(faction.def.factionNameMaker, from fac in Find.FactionManager.AllFactionsVisible
                                                                                    select fac.Name);
            faction.centralMelanin = Rand.Value;
            foreach (Faction item in Find.FactionManager.AllFactions)
            {
                faction.TryMakeInitialRelationsWith(item);
            }

            Pawn p = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier);
            string leaderName = p.Name.ToStringFull;

            faction.GenerateNewLeader();
            faction.leader.Name = new NameSingle(leaderName, true);
            faction.loadID = Find.UniqueIDsManager.GetNextFactionID();

            Find.FactionManager.Add(faction);

            return faction;
        }

        private void ShowInfo()
        {
            DiaNode node = new DiaNode("Quest_SuppressionRebellion_Dia".Translate(Faction.Name));
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public override void Notify_CaravanFormed(QuestSite site, Caravan caravan)
        {
            if(Won)
            {
                site.EndQuest(caravan, EndCondition.Success);
            }
        }

        public override bool PreForceReform(QuestSite mapParent)
        {
            CheckWon();

            return base.PreForceReform(mapParent);
        }

        public override void PostMapRemove(Map map)
        {
            if (!Won)
            {
                Find.LetterStack.ReceiveLetter("Quest_SuppressionRebellionFailTitle".Translate(), "Quest_SuppressionRebellionDesc".Translate(), LetterDefOf.NegativeEvent);

                AttackFail = true;

                Site.Tile = RebelSettlement.Tile;
            }
        }

        private void CheckWon()
        {
            int aliveCount = Site.Map.mapPawns.AllPawnsSpawned.Where(p => p.Faction != null && p.Faction == Faction.OfAncientsHostile && !p.Dead && !p.Downed).Count();

            if (aliveCount > 0)
                return;

            Won = true;
            UnlimitedTime = true;
            Site.RemoveAfterLeave = true;

            int downedCount = Site.Map.mapPawns.AllPawnsSpawned.Where(p => p.Faction != null && p.Faction == Faction.OfAncientsHostile && p.Downed).Count();

            int additionalValue = Mathf.Clamp(downedCount * 70, 150, 1500);
            List<Thing> additionalThings = ThingSetMakerDefOf.Reward_ItemStashQuestContents.root.Generate(new ThingSetMakerParams() { totalMarketValueRange = new FloatRange(additionalValue, additionalValue) });

            StringBuilder builder = new StringBuilder();
            foreach(var thing in additionalThings)
            {
                builder.AppendLine(thing.LabelCap);
                Rewards.Add(thing);
            }

            Find.LetterStack.ReceiveLetter("Quest_SuppressionRebellionWinTitle".Translate(), "Quest_SuppressionRebellionWinDesc".Translate(downedCount, Faction.Name, additionalValue, builder.ToString()), LetterDefOf.PositiveEvent);
        }
    }
}
