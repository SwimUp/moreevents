using MoreEvents.Events;
using MoreEvents.Quests;
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
    public class CaravanArrivalAction_EnterToKillOrderMap : CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(125, 1, 125);

        public Quest_KillOrder Quest => quest;
        private Quest_KillOrder quest;

        public override string Label => "Quest_KillOrder_CardLabel".Translate();

        public override string ReportString => "Quest_KillOrder_CardLabel".Translate();

        public CaravanArrivalAction_EnterToKillOrderMap()
        {
        }

        public CaravanArrivalAction_EnterToKillOrderMap(MapParent mapParent, Quest_KillOrder quest) : base(mapParent)
        {
            this.quest = quest;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref quest, "Quest");
        }

        public override Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            if (map == null)
            {
                map = Verse.MapGenerator.GenerateMap(mapSize, MapParent, MapGeneratorDefOfLocal.EmptyMap);
            }
            return map;
        }
    }
}
