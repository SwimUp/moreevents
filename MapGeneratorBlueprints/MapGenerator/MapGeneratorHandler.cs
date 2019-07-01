﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MapGeneratorBlueprints.MapGenerator
{
    public static class MapGeneratorHandler
    {
        public static void GenerateMap(MapGeneratorDef mapGenerator, Map map, bool clearMap = false, bool setTerrain = false, bool fog = true, bool unFogRoom = false, bool spawnPawns = true
            , bool createRoof = false, bool generatePlants = false, Faction forceFaction = null)
        {
            map.regionAndRoomUpdater.Enabled = false;
            if (clearMap)
            {
                Thing.allowDestroyNonDestroyable = true;

                foreach(var position in map.AllCells)
                {
                    if(position.Roofed(map))
                        map.roofGrid.SetRoof(position, null);
                }

                foreach (Thing item5 in map.listerThings.AllThings.ToList())
                {
                    item5.Destroy();
                }

                Thing.allowDestroyNonDestroyable = false;
            }

            if(generatePlants)
            {
                GenStep genStep = (GenStep)Activator.CreateInstance(typeof(GenStep_Plants));
                genStep.Generate(map, default(GenStepParams));
            }

            if (setTerrain)
            {
                ClearCells(mapGenerator.MapData, map);
                SetTerrain(mapGenerator.MapData, map);
            }

            PlaceBuildingsAndItems(mapGenerator.MapData, map);

            if (spawnPawns)
                SpawnPawns(mapGenerator.MapData, map, forceFaction);

            map.powerNetManager.UpdatePowerNetsAndConnections_First();

            map.regionAndRoomUpdater.Enabled = true;
            map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();

            if (createRoof && mapGenerator.RoofData != null)
            {
                CreateRoof(mapGenerator.RoofData, map);
            }

            if (fog)
            {
                RefogMap(map);
            }

            if (unFogRoom)
            {
                foreach (IntVec3 current in map.AllCells)
                {
                    Room room = current.GetRoom(map);
                    if (room != null && !room.TouchesMapEdge)
                    {
                        map.fogGrid.Unfog(current);
                    }
                }
            }
        }

        private static void ClearCells(List<MapObject> mapObjects, Map map)
        {
            foreach(var obj in mapObjects)
            {
                foreach(var pos in obj.value)
                {
                    List<Thing> tl = map.thingGrid.ThingsListAt(pos);
                    if (tl != null)
                    {
                        for(int i = 0; i < tl.Count; i++)
                        {
                            if(tl[i].def.destroyable)
                            {
                                tl[i].Destroy();
                            }
                        }
                    }
                }
            }
        }

        private static void AddRoomCentersToRootsToUnfog(List<Room> allRooms)
        {
            if (Current.ProgramState != ProgramState.MapInitializing)
            {
                return;
            }
            List<IntVec3> rootsToUnfog = Verse.MapGenerator.rootsToUnfog;
            for (int i = 0; i < allRooms.Count; i++)
            {
                rootsToUnfog.Add(allRooms[i].Cells.RandomElement());
            }
        }

        public static void RefogMap(Map map)
        {
            CellIndices cellIndices = map.cellIndices;
            if (map.fogGrid == null)
            {
                map.fogGrid.fogGrid = new bool[cellIndices.NumGridCells];
            }
            foreach (IntVec3 allCell in map.AllCells)
            {
                map.fogGrid.fogGrid[cellIndices.CellToIndex(allCell)] = true;
            }
            if (Current.ProgramState == ProgramState.Playing)
            {
                map.roofGrid.Drawer.SetDirty();
            }

            foreach (IntVec3 allCell in map.AllCells)
            {
                map.mapDrawer.MapMeshDirty(allCell, MapMeshFlag.FogOfWar);
            }
            FloodFillerFog.FloodUnfog(CellFinder.RandomEdgeCell(map), map);
        }

        private static void AddRoomsToFog(List<Room> allRooms, Map map, bool fogDoors = false)
        {
            CellIndices cellIndices = map.cellIndices;
            foreach (var room in allRooms)
            {
                foreach (var cell in room.Cells)
                {
                    if (cell.GetDoor(map) != null && !fogDoors)
                    {
                        continue;
                    }

                    map.fogGrid.fogGrid[cellIndices.CellToIndex(cell)] = true;
                    map.mapDrawer.MapMeshDirty(cell, MapMeshFlag.FogOfWar);
                }
            }
        }
        public static void CreateRoof(List<RoofObject> roofData, Map map)
        {
            foreach(var roof in roofData)
            {
                foreach(var roofPos in roof.Positions)
                {
                    map.roofGrid.SetRoof(roofPos, roof.RoofDef);
                }
            }
        }

        public static void SpawnPawns(List<MapObject> mapObjects, Map map, Faction forceFaction)
        {
            foreach (var thing in mapObjects)
            {
                ThingData data = thing.key;

                if (data.Kind != null)
                {
                    Faction faction = forceFaction;
                    if(faction == null)
                        faction = Find.FactionManager.FirstFactionOfDef(data.Faction);

                    if (faction != null)
                    {
                        foreach (var pos in thing.value)
                        {
                            Pawn pawn = PawnGenerator.GeneratePawn(data.Kind, faction);

                            if (pawn.RaceProps.Animal && data.Faction == null)
                                pawn.SetFaction(null);
                            if (pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
                                pawn.SetFaction(Faction.OfInsects);

                            pawn = GenSpawn.Spawn(pawn, pos, map) as Pawn;

                            LordJob_DefendPoint lordJob = new LordJob_DefendPoint(pawn.Position);
                            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map);
                            lord.numPawnsLostViolently = int.MaxValue;

                            lord.AddPawn(pawn);
                        }
                    }
                    else
                    {
                        foreach (var pos in thing.value)
                        {
                            Pawn pawn = PawnGenerator.GeneratePawn(data.Kind, faction);
                            pawn = GenSpawn.Spawn(pawn, pos, map) as Pawn;
                        }
                    }
                }
            }
        }


        public static void PlaceBuildingsAndItems(List<MapObject> mapObjects, Map map)
        {
            foreach (var thing in mapObjects)
            {
                ThingData data = thing.key;

                if (data.Thing != null)
                {
                    foreach (var pos in thing.value)
                    {
                        Thing newThing = newThing = ThingMaker.MakeThing(data.Thing, data.Stuff);
                        var comp = newThing.TryGetComp<CompQuality>();
                        if (comp != null)
                        {
                            comp.SetQuality(data.Quality, ArtGenerationContext.Colony);
                        }
                        if (newThing.def.stackLimit != 1)
                            newThing.stackCount = data.Count;
                        GenSpawn.Spawn(newThing, pos, map, data.Rotate);
                    }
                }
            }
        }

        public static void SetTerrain(List<MapObject> mapObjects, Map map)
        {
            foreach (var thing in mapObjects)
            {
                ThingData data = thing.key;

                if (data.Terrain != null)
                {
                    foreach (var pos in thing.value)
                    {
                        map.terrainGrid.SetTerrain(pos, data.Terrain);
                    }
                }
            }
        }
    }
}
