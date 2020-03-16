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
                Log.Message($"POS CLICK --> {UI.MouseCell()}");
                var net = Find.CurrentMap.GetComponent<GasManager>().PipeNetAt(UI.MouseCell());
                if (net != null)
                {
                    Log.Message($"===== NET =====");
                    Log.Message($"Type --> {net.NetType}");
                    Log.Message($"Can push gas --> {net.CanPush}");
                    Log.Message($"Pipes total --> {net.PipesThings.Count}");
                    Log.Message($"Gas plants --> {net.GasPlants.Count}");
                    Log.Message($"Gas coolers --> {net.GasCoolers.Count}");
                    Log.Message($"Gas tankers --> {net.GasTankers.Count}");
                    Log.Message($"===============");
                }

                Find.TickManager.TogglePaused();

            }
        }
    }
}
