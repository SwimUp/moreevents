using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class LightingDestroyEffect_ClimateBombComp : ThingComp
    {
        private List<IntVec3> positions = new List<IntVec3>();
        private int lightingCount = 0;

        private int lightingTimer = 0;
        private int lightingCooldown = 200;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            Map map = parent.Map;

            positions = map.AllCells.Where(cell => !cell.Fogged(map) && cell.Walkable(map) && cell.Standable(map)).ToList();
            lightingCount = Rand.Range(25, 80);
            lightingTimer = lightingCooldown;
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            positions.Clear();
            lightingCount = 0;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (lightingCount > 0)
            {
                lightingTimer--;

                if (lightingTimer <= 0)
                {
                    lightingTimer = lightingCooldown;
                    lightingCount--;

                    Find.CurrentMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(parent.Map, positions.RandomElement()));
                }
            }
            else
            {
                parent.AllComps.Remove(this);
            }
        }
    }
}
