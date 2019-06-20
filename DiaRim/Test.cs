using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DiaRim
{
    public class TestDialogs : GameComponent
    {
        public TestDialogs()
        {

        }

        public TestDialogs(Game game)
        {

        }

        public override void GameComponentTick()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Find.WindowStack.Add(new TestDialogsWindow());
            }
        }

        public class TestDialogsWindow : Window
        {
            private Vector2 scrollPosition = Vector2.zero;
            public override Vector2 InitialSize => new Vector2(200, 400);

            private int defSize = 0;

            public TestDialogsWindow()
            {
                resizeable = false;

                defSize = DefDatabase<DialogDef>.AllDefsListForReading.Count;
            }

            public override void DoWindowContents(Rect inRect)
            {
                int size = defSize * 25;

                Text.Font = GameFont.Small;

                int y = 0;
                Rect scrollRectFact = new Rect(0, 0, 190, 390);
                Rect scrollVertRectFact = new Rect(0, 0, scrollRectFact.x, size);
                Widgets.BeginScrollView(scrollRectFact, ref scrollPosition, scrollVertRectFact);

                foreach (var def in DefDatabase<DialogDef>.AllDefs)
                {
                    if (Widgets.ButtonText(new Rect(0, y, 170, 20), def.defName))
                    {
                        Dialog dia = new Dialog(def);
                        dia.Init();
                        dia.Show();
                    }
                    y += 22;
                }

                Widgets.EndScrollView();
            }
        }
    }
}
