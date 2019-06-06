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
        public List<Thing> items = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref items, "containerItems", LookMode.Deep);
        }

        public override void SpawnSetup(Verse.Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public void AddItem(ThingDef item, int count)
        {
            Thing make = ThingMaker.MakeThing(item);
            make.stackCount = count;

            items.Add(make);
        }

        public void AddItem(Thing item)
        {
            items.Add(item);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach(var pair in items)
            {
                GenDrop.TryDropSpawn(pair, this.Position, this.Map, ThingPlaceMode.Near, out Thing t);
            }

            items.Clear();

            base.Destroy(mode);
        }
    }
}
