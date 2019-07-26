using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public class RandomPlacesWorldComponent : WorldComponent
    {
        private RandomPlacesHandler handler => RandomPlacesComponent.RandomPlacesHandler;

        public RandomPlacesWorldComponent(World world) : base(world)
        {

        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            if (handler == null)
                return;

            if (handler.Triggers.Count == 0)
                return;

            if (Find.TickManager.TicksGame % 100 == 0)
            {
                foreach (var caravan in Find.WorldObjects.Caravans)
                {
                    if(handler.Triggers.ContainsKey(caravan.Tile))
                    {
                        handler.Triggers[caravan.Tile].DoAction(caravan);
                        handler.Triggers.Remove(caravan.Tile);
                    }
                }
            }
        }
    }
}
