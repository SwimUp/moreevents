using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RandomPlaces
{
    public class CompTriggerGenNewMap : ThingComp
    {
        public virtual int CollectThingsRadius => 6;

        public virtual bool ClearMap => true;
        public virtual bool SetTerrain => true;
        public virtual bool Fog => true;
        public virtual bool UnFogRooms => true;
        public virtual bool SpawnPawns => true;
        public virtual bool CreateRoof => true;
        public virtual bool GeneratePlants => true;
        public virtual Faction forceFaction => null;
        public virtual Lord forceLord => null;
        public virtual bool BreakdownBuildings => false;

        public virtual bool OneUse => false;

        private RandomPlaceWorldObject mapHolder;
        private MapGeneratorBlueprints.MapGenerator.MapGeneratorDef generator;

        public virtual MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapGenDefGetter { get; }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (mapHolder != null && mapHolder.HasMap)
            {
                yield return EnterOption(selPawn, false);
            }
            else
            {
                yield return EnterOption(selPawn, true);
            }
        }

        public virtual void PreMapGenerate(RandomPlaceWorldObject worldObject)
        {

        }

        public virtual void PostMapGenerate(Map map, RandomPlaceWorldObject worldObject)
        {

        }

        public virtual void PostMapDefGenerate(Map map, RandomPlaceWorldObject worldObject)
        {

        }

        private FloatMenuOption EnterOption(Pawn pawn, bool first)
        {
            FloatMenuOption option = new FloatMenuOption("CheckInsideLuke".Translate(), null);

            if (!GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, CollectThingsRadius, false).Where(x => x.Faction == Faction.OfPlayer).Any())
            {
                return option;
            }

            option.action = delegate
            {
                DiaOption diaOption = new DiaOption("CheckInsideLuke_OK".Translate());
                diaOption.resolveTree = true;
                diaOption.action = () => DoEnter(first);

                DiaOption diaOption2 = new DiaOption("CheckInsideLuke_NO".Translate());
                diaOption2.resolveTree = true;

                DiaNode diaNode = new DiaNode("CheckInsideLuke_Info".Translate(CollectThingsRadius));
                diaNode.options.Add(diaOption);
                diaNode.options.Add(diaOption2);

                Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode, delayInteractivity: true);

                Find.WindowStack.Add(dialog_NodeTree);
            };

            return option;
        }

        private void DoEnter(bool first)
        {
            if (first)
            {
                generator = MapGenDefGetter;

                IntVec3 mapSize = new IntVec3(generator.size.x, 1, generator.size.z);

                mapHolder = (RandomPlaceWorldObject)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.RandomPlace);
                mapHolder.Tile = parent.Tile;
                Find.WorldObjects.Add(mapHolder);

                PreMapGenerate(mapHolder);

                Map map = null;
                LongEventHandler.QueueLongEvent(delegate
                {
                    map = Verse.MapGenerator.GenerateMap(mapSize, mapHolder, MapGeneratorDefOfLocal.EmptyMap);

                    LongEventHandler.ExecuteWhenFinished(delegate
                    {
                        PostMapGenerate(map, mapHolder);

                        MapGeneratorHandler.GenerateMap(generator, map, out List<Pawn> pawns, ClearMap, SetTerrain, Fog, UnFogRooms, SpawnPawns, CreateRoof, GeneratePlants, forceFaction, forceLord, BreakdownBuildings);

                        var things = GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, CollectThingsRadius, false).Where(x => x.Faction == Faction.OfPlayer);
                        foreach (var t in things)
                        {
                            t.DeSpawn();

                            GenSpawn.Spawn(t, generator.PawnsSpawnPos, map);
                        }

                        PostMapDefGenerate(map, mapHolder);

                        if (OneUse)
                            parent.Destroy();
                    });

                }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
            }
            else
            {
                var things = GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, CollectThingsRadius, false).Where(x => x.Faction == Faction.OfPlayer);
                foreach (var t in things)
                {
                    t.DeSpawn();

                    GenSpawn.Spawn(t, generator.PawnsSpawnPos, mapHolder.Map);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look(ref mapHolder, "mapHolder");
            Scribe_Defs.Look(ref generator, "generator");
        }
    }
}
