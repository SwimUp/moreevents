using RimOverhaul.Gas;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MapTools
{
    public class MapToolsComponent : GameComponent
    {
        public MapToolsComponent()
        {

        }

        public MapToolsComponent(Game game)
        {

        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (Input.GetKeyDown(KeyCode.F10))
                Find.WindowStack.Add(new ThingMapToolWindow());

            if (Input.GetKeyDown(KeyCode.F11))
                Find.WindowStack.Add(new TerrainToolWindow());

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                Log.Message($"RES --> {Find.CurrentMap.GetComponent<GasManager>().PipeAt(UI.MouseCell(), PipeType.NaturalGas)}");
            }
        }
    }
}
