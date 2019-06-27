using MapGeneratorBlueprints.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class CaravanArrivalAction_Doomsday : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(250, 1, 250);

        public CaravanArrivalAction_Doomsday(MapParent mapParent) : base(mapParent)
        {
        }

        public override FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
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
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, Site).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {Site.Label}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {Site.Label}", "LetterCaravanEnteredMap".Translate(caravan.Label, Site).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            IntVec3 enterPos = new IntVec3(82, 0, 5);
            CaravanEnterMapUtility.Enter(caravan, map, (Pawn p) => enterPos);
        }
    }
}
