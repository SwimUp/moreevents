using MoreEvents;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DarkNET.Events.DarkNetSupplyAttack
{
    public class Quest_DarkNetSupplyAttack : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => "Quest_DarkNetSupplyAttack_CardLabel".Translate(TraderDef.LabelCap);

        public override string Description => "Quest_DarkNetSupplyAttack_Description".Translate(TraderDef.LabelCap);

        public override string PlaceLabel => "Quest_DarkNetSupplyAttack_CardLabel".Translate(TraderDef.LabelCap);

        public DarkNetTraderDef TraderDef;

        public CommunicationDialog CommunicationDialog;

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

        public override Texture2D ExpandingIcon => DarkNetTrader.IconMenu;

        private DarkNetTrader darkNetTrader;

        public Quest_DarkNetSupplyAttack() : base()
        {

        }

        public Quest_DarkNetSupplyAttack(DarkNetTraderDef traderDef)
        {
            TraderDef = traderDef;
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            if (Rewards != null)
            {
                float points = 0;
                foreach (var item in Rewards)
                {
                    points += item.MarketValue * item.stackCount;

                    var pos = CellFinder.RandomClosewalkCellNear(map.Center, map, 5, x => x.Walkable(map) && !x.Fogged(map));
                    if(pos != IntVec3.Invalid)
                    {
                        GenDrop.TryDropSpawn(item, pos, map, ThingPlaceMode.Near, out Thing result);
                    }
                }

                points *= 0.6f;

                GeneratePawns(map, points);

                Rewards.Clear();
            }
        }

        private void GeneratePawns(Map map, float points)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = points,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            var pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            IncidentParms parms = new IncidentParms
            {
                target = map,
                spawnCenter = CellFinder.RandomClosewalkCellNear(map.Center, map, 15, x => x.Walkable(map) && !x.Fogged(map))
            };

            PawnsArrivalModeDefOf.CenterDrop.Worker.Arrive(pawns, parms);

            LordJob_AssaultColony lordJob_AssaultColony = new LordJob_AssaultColony(Faction);
            Lord lord = LordMaker.MakeNewLord(Faction, lordJob_AssaultColony, map, pawns);

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
        }
    }
}
