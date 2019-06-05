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
        private Dictionary<Thing, int> items = new Dictionary<Thing, int>();

        public override void SpawnSetup(Verse.Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public void AddItem(ThingDef item, int count)
        {
            Thing make = ThingMaker.MakeThing(item);

            if (items.Keys.Contains(make))
            {
                items[make] += count;
            }
            else
            {
                items.Add(make, count);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach(var pair in items)
            {
                GenDrop.TryDropSpawn(pair.Key, this.Position, this.Map, ThingPlaceMode.Near, out Thing t);

                t.stackCount = pair.Value;
            }

            items.Clear();

            base.Destroy(mode);
        }
    }
}
