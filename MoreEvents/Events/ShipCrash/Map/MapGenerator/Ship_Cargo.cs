using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public abstract class Ship_Cargo : ShipMapGenerator
    {
        protected EventSettings settings => Settings.EventsSettings["ShipCrash"];

        public enum CargoType : byte
        {
            Complex = 0, //все вместе
            Mining, //ресурсы, добываемые
            Food //еда
        }

        public override ShipSiteType SiteType => ShipSiteType.Cargo;

        public abstract CargoType PartType { get; }
    }
}
