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
    public class IncidentWorker_ShipCrash : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["ShipCrash"];

        private int minParts => int.Parse(settings.Parameters["MinParts"].Value);
        private int maxParts => int.Parse(settings.Parameters["MaxParts"].Value);

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (ShipCrash_Controller.ShipCount == ShipCrash_Controller.MaxShips)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<int> tiles = new List<int>();
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                Tile tile = Find.WorldGrid[i];
                if (tile.biome != BiomeDefOf.Ocean && tile.biome != BiomeDefOf.Lake && tile.hilliness != Hilliness.Impassable)
                {
                    tiles.Add(i);
                }
            }

            if (tiles.Count == 0)
                return false;

            int partsCount = Rand.Range(minParts, maxParts);

            for (int i = 0; i < partsCount; i++)
            {
                int tileID = tiles.RandomElement();

                if (Find.WorldObjects.AnyMapParentAt(tileID))
                    continue;

                Faction f = RandomEnemyFaction();
                if (f == null)
                    return false;

                ShipMapGenerator generator = GetGenerator();
                ShipCrash_Controller.MakeShipPart(generator, tileID, f);
            }

            Find.LetterStack.ReceiveLetter(def.label.Translate(), def.letterText.Translate(), LetterDefOf.NegativeEvent);

            ShipCrash_Controller.ShipCount++;

            return true;
        }

        private Faction RandomEnemyFaction(bool allowHidden = false, bool allowDefeated = false, bool allowNonHumanlike = true, TechLevel minTechLevel = TechLevel.Undefined)
        {
            var list = from x in Find.FactionManager.AllFactions
                       where !x.IsPlayer && (allowHidden || !x.def.hidden) && (allowDefeated || !x.defeated) && (allowNonHumanlike || x.def.humanlikeFaction) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel) && x.HostileTo(Faction.OfPlayer)
                       select x;

            List<Faction> tmpList = new List<Faction>();

            foreach(var l in list)
            {
                var kinds = l.def.pawnGroupMakers;
                if (kinds == null)
                    continue;

                foreach(var kind in kinds)
                {
                    if(kind.kindDef == PawnGroupKindDefOf.Combat)
                    {
                        tmpList.Add(l);
                        continue;
                    }
                }
            }

            if (tmpList.Count == 0)
                return null;

            return tmpList.RandomElement();
        }

        private ShipMapGenerator GetGenerator()
        {
            while(true)
            {
                if(Rand.Chance(0.21f))
                {
                    return new ShipCargo_Food();
                }
                if(Rand.Chance(0.12f))
                {
                    return new Ship_Living();
                }
                if (Rand.Chance(0.23f))
                {
                    return new ShipCargo_Mining();
                }
                if(Rand.Chance(0.14f))
                {
                    return new ShipCargo_Complex();
                }
                if(Rand.Chance(0.11f))
                {
                    return new Ship_Armory();
                }
            }
        }
    }
}
