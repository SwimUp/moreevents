using DarkNET.Quests;
using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Caravans
{
    public class CaravanArrivalAction_EnterToLaboratoryMap : CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(250, 1, 250);

        public Quest_Laboratory Quest => quest;
        private Quest_Laboratory quest;

        public override string Label => "Quest_Laboratory_CardLabel".Translate();

        public override string ReportString => "Quest_Laboratory_CardLabel".Translate();

        public CaravanArrivalAction_EnterToLaboratoryMap()
        {
        }

        public CaravanArrivalAction_EnterToLaboratoryMap(MapParent mapParent, Quest_Laboratory quest) : base(mapParent)
        {
            this.quest = quest;
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

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref quest, "Quest");
        }

        public override void DoEnter(Caravan caravan)
        {
            Pawn t = caravan.PawnsListForReading[0];
            bool flag2 = !MapParent.HasMap;
            Verse.Map orGenerateMap = GetOrGenerateMap(MapParent.Tile, MapSize, null);
            if (flag2)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsSite".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, questSite.Label).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {questSite.Label}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {questSite.Label}", "LetterCaravanEnteredMap".Translate(caravan.Label, questSite.Label).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            IntVec3 spawnPos = new IntVec3(33, 0, 4);
            CaravanEnterMapUtility.Enter(caravan, map, (Pawn p) => spawnPos);
        }
    }
}
