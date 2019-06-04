using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.ShipCrash
{
    public class Building_Container : Building
    {
        private Dictionary<int, Thing> items = new Dictionary<int, Thing>();

        public override void SpawnSetup(Verse.Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public void AddItem(ThingDef item, int count)
        {
            items.Add(count, ThingMaker.MakeThing(item));
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach(var pair in items)
            {
                GenDrop.TryDropSpawn(pair.Value, this.Position, this.Map, ThingPlaceMode.Near, out Thing t);

                t.stackCount = pair.Key;
            }

            items.Clear();

            base.Destroy(mode);
        }
    }
}
