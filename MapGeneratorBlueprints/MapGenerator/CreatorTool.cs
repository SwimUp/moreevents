using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using MapGenerator;
using RimWorld;

namespace MapGeneratorBlueprints.MapGenerator
{
    public class CreatorTool : GameComponent
    {
        public CreatorTool()
        {

        }

        public CreatorTool(Game game)
        {

        }

        public override void GameComponentTick()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Find.WindowStack.Add(new TestWindow());
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Find.WindowStack.Add(new SaveMapWindow());
            }
        }

        public class SaveMapWindow : Window
        {
            public bool SavePlants;
            public bool SaveTerrain;
            public bool SavePawns;
            public bool SaveRoof;
            public string FileName;

            public override Vector2 InitialSize => new Vector2(450, 400);

            public override void DoWindowContents(Rect inRect)
            {
                if(Widgets.RadioButtonLabeled(new Rect(10,10, 370, 20), "Save plants?", SavePlants))
                {
                    SavePlants = !SavePlants;
                }

                if (Widgets.RadioButtonLabeled(new Rect(10, 40, 370, 20), "Save terrain?", SaveTerrain))
                {
                    SaveTerrain = !SaveTerrain;
                }

                if (Widgets.RadioButtonLabeled(new Rect(10, 70, 370, 20), "Save Pawns?", SavePawns))
                {
                    SavePawns = !SavePawns;
                }

                if (Widgets.RadioButtonLabeled(new Rect(10, 100, 370, 20), "Save Roof?", SaveRoof))
                {
                    SaveRoof = !SaveRoof;
                }

                FileName = Widgets.TextField(new Rect(10, 130, 370, 20), FileName);

                if(Widgets.ButtonText(new Rect(10, 160, 370, 20), "Create"))
                {
                    if (!string.IsNullOrEmpty(FileName))
                        SaveMap(FileName, SavePlants, SaveTerrain, SavePawns, SaveRoof);
                }
            }
        }

        public static void SaveMap(string fileName, bool savePlants = true, bool saveTerrain = true, bool savePawns = true, bool saveRoof = false)
        {
            MapGeneratorData newDef = new MapGeneratorData();
            newDef.MapData = new List<MapObject>();
            newDef.RoofData = new List<RoofObject>();
            newDef.defName = fileName;
            newDef.size = Find.CurrentMap.Size.ToIntVec2;

            foreach(var thing in Find.CurrentMap.listerThings.AllThings)
            {
                if(savePawns && thing.def.category == ThingCategory.Pawn)
                {
                    Pawn p = thing as Pawn;
                    if (p != null)
                    {
                        MapObject containedObject = newDef.MapData.Where(x => x.key.Kind == p.kindDef && x.key.Faction == p.Faction?.def).FirstOrDefault();
                        if (containedObject != null)
                        {
                            containedObject.value.Add(thing.Position);
                        }
                        else
                        {
                            MapObject newData = new MapObject();
                            newData.key = new ThingData()
                            {
                                Faction = p.Faction?.def,
                                Kind = p.kindDef
                            };
                            newData.value = new List<IntVec3>
                            {
                            thing.Position
                            };

                            newDef.MapData.Add(newData);
                        }
                    }
                }

                if(thing.def.category == ThingCategory.Building || (savePlants && thing.def.category == ThingCategory.Plant))
                {
                    MapObject containedObject = newDef.MapData.Where(x => x.key.Thing == thing.def && x.key.Stuff == thing.Stuff).FirstOrDefault();
                    if(containedObject != null)
                    {
                        containedObject.value.Add(thing.Position);
                    }
                    else
                    {
                        MapObject newData = new MapObject();
                        newData.key = new ThingData()
                        {
                            Thing = thing.def,
                            Stuff = thing.Stuff
                        };
                        newData.value = new List<IntVec3>
                        {
                            thing.Position
                        };

                        newDef.MapData.Add(newData);
                    }
                }

                if (thing.def.category == ThingCategory.Item)
                {
                    var comp = thing.TryGetComp<CompQuality>();
                    MapObject containedObject = newDef.MapData.Where(x => x.key.Thing == thing.def && x.key.Stuff == thing.Stuff && (comp != null && comp.Quality == x.key.Quality || comp == null) &&
                    x.key.Count == thing.stackCount).FirstOrDefault();
                    if (containedObject != null)
                    {
                        containedObject.value.Add(thing.Position);
                    }
                    else
                    {
                        MapObject newData = new MapObject();
                        newData.key = new ThingData()
                        {
                            Thing = thing.def,
                            Stuff = thing.Stuff,
                            Count = thing.stackCount,
                        };
                        if(comp != null)
                        {
                            newData.key.Quality = comp.Quality;
                        }

                        newData.value = new List<IntVec3>
                        {
                            thing.Position
                        };

                        newDef.MapData.Add(newData);
                    }
                }
            }

            if (saveTerrain)
            {
                foreach (var position in Find.CurrentMap.AllCells)
                {
                    TerrainDef terrain = position.GetTerrain(Find.CurrentMap);
                    if (terrain != null)
                    {
                        MapObject containedObject = newDef.MapData.Where(x => x.key.Terrain == terrain).FirstOrDefault();
                        if (containedObject != null)
                        {
                            containedObject.value.Add(position);
                        }
                        else
                        {
                            MapObject newData = new MapObject();
                            newData.key = new ThingData()
                            {
                                Terrain = terrain
                            };

                            newData.value = new List<IntVec3>
                            {
                            position
                            };

                            newDef.MapData.Add(newData);
                        }
                    }
                }
            }

            if(saveRoof)
            {
                foreach(var position in Find.CurrentMap.AllCells)
                {
                    RoofDef roofDef = position.GetRoof(Find.CurrentMap);
                    if(roofDef != null)
                    {
                        RoofObject containedObject = newDef.RoofData.Where(x => x.RoofDef == roofDef).FirstOrDefault();
                        if (containedObject != null)
                        {
                            containedObject.Positions.Add(position);
                        }
                        else
                        {
                            RoofObject newData = new RoofObject();
                            newData.RoofDef = roofDef;
                            newData.Positions = new List<IntVec3>
                            {
                                position
                            };

                            newDef.RoofData.Add(newData);
                        }
                    }
                }
            }

            Scribe.saver.InitSaving($@"{fileName}.xml", "Defs");
            Scribe_Deep.Look(ref newDef, "MapGeneratorBlueprints.MapGenerator.MapGeneratorDef");
            Scribe.saver.FinalizeSaving();
        }


        public class TestWindow : Window
        {
            private Vector2 scrollPosition = Vector2.zero;
            public override Vector2 InitialSize => new Vector2(210, 400);

            private int defSize = 0;

            public bool Fog = true;
            public bool RoomFog = true;
            public bool Terrain = false;
            public bool Pawns = true;
            public bool Roof = true;

            public TestWindow()
            {
                resizeable = false;

                defSize = DefDatabase<MapGeneratorDef>.AllDefsListForReading.Count;
            }

            public override void DoWindowContents(Rect inRect)
            {
                int size = defSize * 25;

                Text.Font = GameFont.Small;

                if (Widgets.RadioButtonLabeled(new Rect(0, 0, 170, 20), "Fog", Fog))
                    Fog = !Fog;

                if (Widgets.RadioButtonLabeled(new Rect(0, 40, 170, 20), "UnFogRooms", RoomFog))
                    RoomFog = !RoomFog;

                if (Widgets.RadioButtonLabeled(new Rect(0, 80, 170, 20), "Set Terrain", Terrain))
                    Terrain = !Terrain;

                if (Widgets.RadioButtonLabeled(new Rect(0, 110, 170, 20), "Spawn pawns", Pawns))
                    Pawns = !Pawns;

                if (Widgets.RadioButtonLabeled(new Rect(0, 140, 170, 20), "Spawn roof", Roof))
                    Roof = !Roof;

                int y = 0;
                Rect scrollRectFact = new Rect(0, 170, 190, 200);
                Rect scrollVertRectFact = new Rect(0, 0, scrollRectFact.x, size);
                Widgets.BeginScrollView(scrollRectFact, ref scrollPosition, scrollVertRectFact);

                foreach (var def in DefDatabase<MapGeneratorDef>.AllDefs)
                {
                    if (Widgets.ButtonText(new Rect(0, y, 170, 20), def.defName))
                    {
                        MapGeneratorHandler.GenerateMap(def, Find.CurrentMap, true, Terrain, Fog, RoomFog, Pawns, Roof);
                    }
                    y += 22;
                }

                Widgets.EndScrollView();
            }
        }
    }
}
