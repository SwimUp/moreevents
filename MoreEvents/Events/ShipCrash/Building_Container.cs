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
        private Dictionary<ThingDef, int> items = new Dictionary<ThingDef, int>();

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref items, "containerItems", LookMode.Def, LookMode.Value);
        }

        public override void SpawnSetup(Verse.Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public void AddItem(ThingDef item, int count)
        {
            if (items.Keys.Contains(item))
            {
                items[item] += count;
            }
            else
            {
                items.Add(item, count);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach(var pair in items)
            {
                Thing make = ThingMaker.MakeThing(pair.Key);
                GenDrop.TryDropSpawn(make, this.Position, this.Map, ThingPlaceMode.Near, out Thing t);

                t.stackCount = pair.Value;
            }

            items.Clear();

            base.Destroy(mode);
        }
    }
}
