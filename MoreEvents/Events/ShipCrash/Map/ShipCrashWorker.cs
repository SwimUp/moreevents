using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map
{
    public class ShipCrashWorker : WorldObjectComp
    {
        public ShipSiteType SiteType => Generator.SiteType;

        public ShipMapGenerator Generator { get; private set; }

        public void InitWorker(ShipMapGenerator generator)
        {
            Generator = generator;
        }

        public override void PostMapGenerate()
        {
            Generator.RunGenerator(this, (parent as MapParent).Map, parent.Faction);
        }
    }
}
