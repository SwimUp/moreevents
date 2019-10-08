using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimOverhaul;
using RimOverhaul.Quests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DarkNET.Quests
{
    public class Quest_Archotech_567H_GetResources : Quest_Archotech_567H
    {
        public override string ExpandingIconPath => "Quests/Quest_Archotech_567H_GetResources";

        public override string CardLabel => "Quest_Archotech_567H_GetResources_CardLabel".Translate();

        public override string Description => "Quest_Archotech_567H_GetResources_Description".Translate();

        public override string PlaceLabel => "Quest_Archotech_567H_GetResources_PlaceLabel".Translate();

        public override string MapTargetTag => "Quest_Archotech_567H_GetResources";

        public override float QuestChance => 0.3f;

        public override Faction FactionGetter => Find.FactionManager.RandomEnemyFaction();

        public override FloatRange RewardRange => new FloatRange(1500, 3000);

        public override IntRange CountRange => new IntRange(4, 10);

        public bool Defended = false;

        private bool signalSended = false;
        private int ticks = 40000;

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if(!Defended)
            {
                Messages.Message(Translator.Translate("Quest_Archotech_567H_GetResources_NeedDefend"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (HostileUtility.AnyNonDownedHostileOnMap(site.Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            Won = true;

            return true;
        }

        public override IEnumerable<Gizmo> GetGizmos(QuestSite site)
        {
            foreach(var gizmo in base.GetGizmos(site))
            {
                yield return gizmo;
            }

            if (!signalSended && site.HasMap)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Quest_Archotech_567H_GetResources_SignalLabel".Translate(),
                    defaultDesc = "Quest_Archotech_567H_GetResources_SignalDescription".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Quests/send-signal"),
                    action = delegate
                    {
                        signalSended = true;

                        DiaNode node = new DiaNode("Quest_Archotech_567H_GetResources_SignalDialog".Translate());
                        DiaOption option = new DiaOption("OK");
                        node.options.Add(option);
                        option.resolveTree = true;

                        var dialog = new Dialog_NodeTree(node);
                        Find.WindowStack.Add(dialog);

                        List<Pawn> pawns = site.Map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == Faction && !p.Downed && !p.Dead).ToList();

                        LordJob lordJob = new LordJob_AssaultColony(Faction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
                        Lord lord = LordMaker.MakeNewLord(Faction, lordJob, site.Map);
                        lord.numPawnsLostViolently = int.MaxValue;

                        foreach (var p in pawns)
                        {
                            Lord lastLord = p.GetLord();
                            if (lastLord != null)
                            {
                                site.Map.lordManager.RemoveLord(lastLord);
                            }

                            p.ClearMind();
                            lord.AddPawn(p);
                        }
                    }
                };
            }
        }

        public override void SiteTick()
        {
            base.SiteTick();

            if(!Defended && signalSended)
            {
                ticks--;
                if(ticks <= 0)
                {
                    DefenseSuccess();

                    return;
                }
                if(ticks % 10000 == 0)
                {
                    SendRaid();
                }
            }
        }

        private void SendRaid()
        {
            if (!Site.HasMap)
                return;

            Map map = Site.Map;

            int @int = Rand.Int;
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            raidParms.forced = true;
            raidParms.faction = Faction;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            raidParms.points = Rand.Range(200, 450);
            raidParms.pawnGroupMakerSeed = @int;
            var incident = new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms);
            Find.Storyteller.TryFire(incident);
        }

        private void DefenseSuccess()
        {
            Defended = true;

            Find.LetterStack.ReceiveLetter("Quest_Archotech_567H_GetResources_SuccessTitle".Translate(), "Quest_Archotech_567H_GetResources_SuccessDesc".Translate(), LetterDefOf.PositiveEvent);
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            DiaNode node = new DiaNode("Quest_Archotech_567H_GetResources_Dialog".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);
            filter.SetAllow(ThingCategoryDefOf.BodyParts, true);

            return filter;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Defended, "Defended");
            Scribe_Values.Look(ref signalSended, "signalSended");
            Scribe_Values.Look(ref ticks, "ticks");
        }
    }
}
