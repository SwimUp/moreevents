using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MapTools
{
    public class ThingMapToolWindow : EditWindow
    {
        public ThingDef FloodThing;

        public int BrushSize = 2;
        public string BrushBuff;

        public ThingMapToolWindow()
        {
            resizeable = false;
            Thing.allowDestroyNonDestroyable = true;
        }

        public override void PostClose()
        {
            Thing.allowDestroyNonDestroyable = false;

            base.PostClose();

        }

        public override Vector2 InitialSize => new Vector2(210, 200);
        public override void DoWindowContents(Rect inRect)
        {
            if(Widgets.ButtonText(new Rect(0,0, 170, 20), FloodThing?.LabelCap))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("Камни", delegate
                {
                    List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                    foreach (var thing2 in DefDatabase<ThingDef>.AllDefs.Where(t => t.building != null && t.building.isNaturalRock))
                    {
                        list2.Add(new FloatMenuOption(thing2.LabelCap, delegate
                        {
                            FloodThing = thing2;
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list2));
                }));
                foreach (var thing in DefDatabase<ThingCategoryDef>.AllDefsListForReading)
                {
                    list.Add(new FloatMenuOption(thing.LabelCap, delegate
                    {
                        List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                        foreach (var thing2 in DefDatabase<ThingDef>.AllDefs.Where(t => t.IsWithinCategory(thing)))
                        {
                            list2.Add(new FloatMenuOption(thing2.LabelCap, delegate
                            {
                                FloodThing = thing2;
                            }));
                         }

                        Find.WindowStack.Add(new FloatMenu(list2));
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            Widgets.TextFieldNumeric(new Rect(0, 30, 170, 20), ref BrushSize, ref BrushBuff, 1, 1000);
        }

        public override void WindowOnGUI()
        {
            base.WindowOnGUI();

            if(FloodThing != null && Input.GetKeyDown(KeyCode.Mouse0))
            {
                CellRect cellRect2 = CellRect.CenteredOn(UI.MouseCell(), BrushSize);
                cellRect2.ClipInsideMap(Find.CurrentMap);
                foreach (IntVec3 item7 in cellRect2)
                {
                    GenSpawn.Spawn(FloodThing, item7, Find.CurrentMap);
                }
            }
        }
    }

    public class TerrainToolWindow : EditWindow
    {
        public TerrainDef FloodTerrain;

        public int BrushSize = 2;
        public string BrushBuff;

        public TerrainToolWindow()
        {
            resizeable = false;
        }

        public override Vector2 InitialSize => new Vector2(210, 200);
        public override void DoWindowContents(Rect inRect)
        {
            if (Widgets.ButtonText(new Rect(0, 0, 170, 20), FloodTerrain?.LabelCap))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (var thing in DefDatabase<TerrainDef>.AllDefsListForReading)
                {
                    list.Add(new FloatMenuOption(thing.LabelCap, delegate
                    {
                        FloodTerrain = thing;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            Widgets.TextFieldNumeric(new Rect(0, 30, 170, 20), ref BrushSize, ref BrushBuff, 1, 1000);

            if (Widgets.ButtonText(new Rect(0, 60, 170, 20), "Установить покрытие на всю карту"))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("Покрытия", delegate
                {
                    List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                    foreach (var thing2 in DefDatabase<TerrainDef>.AllDefs)
                    {
                        list2.Add(new FloatMenuOption(thing2.LabelCap, delegate
                        {
                            foreach (var cell in Find.CurrentMap.AllCells)
                                Find.CurrentMap.terrainGrid.SetTerrain(cell, thing2);
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list2));
                }));

                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        public override void WindowOnGUI()
        {
            base.WindowOnGUI();

            if (FloodTerrain != null && Input.GetKeyDown(KeyCode.Mouse0))
            {
                CellRect cellRect2 = CellRect.CenteredOn(UI.MouseCell(), BrushSize);
                cellRect2.ClipInsideMap(Find.CurrentMap);
                foreach (IntVec3 item7 in cellRect2)
                {
                    Find.CurrentMap.terrainGrid.SetTerrain(item7, FloodTerrain);
                }
            }
        }
    }
}
