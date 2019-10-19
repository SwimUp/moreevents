using DarkNET.Events.DarkNetSupplyAttack;
using MoreEvents;
using QuestRim;
using RimOverhaul.Gss;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace DarkNET.Events.DarkNetTraderSupply
{
    public class Quest_DarkNetTraderSupply : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => string.Format(IncidentDefOfLocal.DarkNetTraderSupply.letterLabel, TraderDef.LabelCap);

        public override string Description => string.Format(IncidentDefOfLocal.DarkNetTraderSupply.letterLabel, TraderDef.LabelCap, Faction.Name, GarantFaction.Name);

        public override string PlaceLabel => "Quest_DarkNetTraderSupply_CardLabel".Translate();

        public DarkNetTraderDef TraderDef;

        public CommunicationDialog CommunicationDialog;

        public override string ExpandingIconPath => "Quests/Quest_DarkNetTraderSupply";
        public override bool UseLeaveCommand => false;

        public override bool HasExitCells => true;

        public DarkNetTrader DarkNetTrader
        {
            get
            {
                if (darkNetTrader == null)
                {
                    darkNetTrader = Current.Game.GetComponent<DarkNet>().Traders.First(x => x.def == TraderDef);
                }

                return darkNetTrader;
            }
        }

        private DarkNetTrader darkNetTrader;

        public Faction GarantFaction;

        private bool GSSRaid = false;

        public Quest_DarkNetTraderSupply() : base()
        {

        }

        public Quest_DarkNetTraderSupply(DarkNetTraderDef traderDef, Faction garant)
        {
            TraderDef = traderDef;
            GarantFaction = garant;
        }

        public override string GetDescription()
        {
            return $"Quest_DarkNetTraderSupply_GetDescription".Translate(TraderDef.LabelCap, Faction.Name, GarantFaction.Name);
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            float points = GenerateItems(map);

            GeneratePawns(map, points);

            Faction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile);
            GarantFaction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile);

            DarkNetTrader.Block(50);

            if (CommunicationDialog != null)
                QuestsManager.Communications.RemoveCommunication(CommunicationDialog);
        }
        private float GenerateItems(Map map)
        {
            float points = 200;

            if (Rewards != null)
            {
                foreach (var item in Rewards)
                {
                    points += item.MarketValue * item.stackCount;

                    var pos = CellFinder.RandomClosewalkCellNear(map.Center, map, 5, x => x.Walkable(map) && !x.Fogged(map));
                    if (pos != IntVec3.Invalid)
                    {
                        GenDrop.TryDropSpawn(item, pos, map, ThingPlaceMode.Near, out Thing result);
                    }
                }

                points *= 0.6f;

                Rewards.Clear();
            }

            return points;
        }

        public override void PostSiteRemove(QuestSite site)
        {
            base.PostSiteRemove(site);

            EndQuest();
        }
        public override void SiteTick()
        {
            base.SiteTick();

            if(Site.HasMap)
            {
                if(!GSSRaid && Find.TickManager.TicksGame % 10000 == 0)
                {
                    if(Rand.Chance(0.1f))
                    {
                        SendGss();
                    }
                }
            }
        }

        private void SendGss()
        {
            GSSRaid = true;

            if (DarkNet.GssFaction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile)
            {
                DarkNet.GssFaction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile, true, "DarKNet_WhyAffect".Translate());
            }

            GssRaids.SendRaidImmediately(Site.Map, Rand.Range(800, 1400), false);
        }
        private void GeneratePawns(Map map, float points)
        {
            float genPoints = points * 0.7f;

            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out IntVec3 first, out IntVec3 second);

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = genPoints,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            var pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            SpawnAndGiveLord(pawns, Faction, map, first);

            pawnGroupMakerParms.faction = GarantFaction;
            pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            SpawnAndGiveLord(pawns, GarantFaction, map, second);
        }

        private void SpawnAndGiveLord(List<Pawn> pawns, Faction faction, Map map, IntVec3 spot)
        {
            foreach (var firstFactionPawn in pawns)
            {
                if (CellFinder.TryFindRandomCellNear(spot, map, 6, (IntVec3 x) => x.Standable(map) && !x.Fogged(map), out IntVec3 loc))
                {
                    GenSpawn.Spawn(firstFactionPawn, loc, map, Rot4.Random);
                }
            }
            LordJob_AssaultColony lordJob_AssaultColony = new LordJob_AssaultColony(faction);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob_AssaultColony, map, pawns);
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if (CommunicationDialog != null)
                QuestsManager.Communications.RemoveCommunication(CommunicationDialog);

            base.EndQuest(caravan, condition);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToRaidMap caravanAction = new CaravanArrivalAction_EnterToRaidMap(mapParent);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref TraderDef, "TraderDef");
            Scribe_References.Look(ref CommunicationDialog, "CommunicationDialog");
            Scribe_References.Look(ref GarantFaction, "Garant");
            Scribe_Values.Look(ref GSSRaid, "GSSRaid");
        }
    }
}
