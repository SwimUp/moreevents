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
        public static int ShipCount;

        public ShipCrash_Controller()
        {

        }

        public ShipCrash_Controller(Game game)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ShipCount, "ShipCount", 0);
        }

        public static ShipSite MakeShipPart(ShipMapGenerator gen, int tileID, Faction faction)
        {
            ShipSite site = (ShipSite)WorldObjectMaker.MakeWorldObject(ShipWorldObjectDefOf.ShipCrashSite);

            site.SetGenerator(gen);
            site.Tile = tileID;
            site.SetFaction(faction);

            Find.WorldObjects.Add(site);

            return site;
        }
    }
}
