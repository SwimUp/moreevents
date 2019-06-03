using MoreEvents.Events.ShipCrash.Map;
using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ShipCrash
{
    public class ShipCrash_Controller : GameComponent
    {
        public enum ShipSiteType : byte
        {
            Cargo = 0, //грузовой отсек
            Living, //Жилой отсек
            Armory //Оружейная
        }

        public const int MaxShips = 3;
        public static int ShipCount
        {
            get
            {
                return shipCount;
            }
        }
        private static int shipCount;

        public ShipCrash_Controller()
        {

        }

        public ShipCrash_Controller(Verse.Map map)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref shipCount, "ShipCount", 0);
        }

        public static ShipCrashWorker MakeShipPart(ShipMapGenerator gen, int tileID, Faction faction)
        {
            Site site = (Site)WorldObjectMaker.MakeWorldObject(ShipWorldObjectDefOf.ShipCrashSite);
            site.Tile = tileID;
            site.SetFaction(faction);

            var shipComp = site.GetComponent<ShipCrashWorker>();
            shipComp.InitWorker(gen);

            return shipComp;
        }
    }
}
