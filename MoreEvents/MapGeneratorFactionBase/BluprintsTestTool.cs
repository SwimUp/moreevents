using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.MapGeneratorFactionBase
{
    public class BluprintsTestTool : GameComponent
    {
        public BluprintsTestTool()
        {

        }

        public BluprintsTestTool(Game game)
        {

        }

        public override void GameComponentTick()
        {
            if(Input.GetKeyDown(KeyCode.F5))
            {
                Find.WindowStack.Add(new BlueprintsTestWindow());
            }
        }

        public class BlueprintsTestWindow : Window
        {
            private Vector2 scrollPosition = Vector2.zero;
            public override Vector2 InitialSize => new Vector2(200, 400);

            private int defSize = 0;

            public BlueprintsTestWindow()
            {
                Log.Message("OPEN");

                resizeable = false;

                defSize = DefDatabase<MapGeneratorBaseBlueprintDef>.AllDefsListForReading.Count;
            }

            public override void DoWindowContents(Rect inRect)
            {
                int size = defSize * 25;

                Text.Font = GameFont.Small;

                int y = 0;
                Rect scrollRectFact = new Rect(0, 0, 190, 390);
                Rect scrollVertRectFact = new Rect(0, 0, scrollRectFact.x, size);
                Widgets.BeginScrollView(scrollRectFact, ref scrollPosition, scrollVertRectFact);

                foreach (var def in DefDatabase<MapGeneratorBaseBlueprintDef>.AllDefs)
                {
                    if(Widgets.ButtonText(new Rect(0, y, 170, 20), def.defName))
                    {
                        Thing t = Find.Selector.SingleSelectedThing;
                        if (t != null)
                        {
                            BlueprintHandler.CreateBlueprintAt(t.Position, Find.CurrentMap, def, Find.FactionManager.RandomEnemyFaction(), null, null);
                        }
                        else
                        {
                            Log.Message($"THING NULL");
                        }
                    }
                    y += 22;
                }

                Widgets.EndScrollView();
            }
        }
    }
}
