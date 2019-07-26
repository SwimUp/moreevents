using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public class WorldTrigger : IExposable
    {
        public int Tile;
        public Faction Faction;
        public RandomPlaceDef RandomPlaceDef;

        public WorldTrigger()
        {

        }

        public WorldTrigger(int tile, RandomPlaceDef def, Faction faction)
        {
            Tile = tile;
            RandomPlaceDef = def;
            Faction = faction;
        }

        public void DoAction(Caravan caravan)
        {
            RandomPlaceWorldObject obj = (RandomPlaceWorldObject)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.RandomPlace);
            obj.Tile = caravan.Tile;
            obj.SetFaction(Faction);
            obj.RandomPlaceDef = RandomPlaceDef;
            Find.WorldObjects.Add(obj);

            LongEventHandler.QueueLongEvent(delegate
            {
                DoEnter(caravan, obj);
            }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
        }

        private IntVec3 GetMapSize()
        {
            return new IntVec3(RandomPlaceDef.Map.size.x, 1, RandomPlaceDef.Map.size.z);
        }

        public virtual void DoEnter(Caravan caravan, MapParent mapParent)
        {
            Pawn t = caravan.PawnsListForReading[0];
            Verse.Map orGenerateMap = GetOrGenerateMap(Tile, GetMapSize(), mapParent, null);
            Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsSite".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, "EnteredToRandomPlace".Translate()).CapitalizeFirst());
            Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {"EnteredToRandomPlace".Translate()}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            CaravanEnterMode enterMode = CaravanEnterMode.Edge;

            if(RandomPlaceDef.UseMapSpawnSpot)
                CaravanEnterMapUtility.Enter(caravan, orGenerateMap, (Pawn p) => RandomPlaceDef.Map.PawnsSpawnPos, CaravanDropInventoryMode.DoNotDrop);
            else
                CaravanEnterMapUtility.Enter(caravan, orGenerateMap, enterMode, CaravanDropInventoryMode.DoNotDrop);
        }

        public virtual Map GetOrGenerateMap(int tile, IntVec3 mapSize, MapParent mapParent, WorldObjectDef suggestedMapParentDef)
        {
            return Verse.MapGenerator.GenerateMap(mapSize, mapParent, MapGeneratorDefOfLocal.EmptyMap);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Tile, "Tile");
            Scribe_Defs.Look(ref RandomPlaceDef, "RandomPlaceDef");
            Scribe_References.Look(ref Faction, "Faction");
        }
    }
}
