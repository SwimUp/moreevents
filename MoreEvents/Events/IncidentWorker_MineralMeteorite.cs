using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_MineralMeteorite : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MineralMeteorite"];

        private ThingDef[] things = new ThingDef[]
        {
            ThingDefOf.Granite,
            ThingDefOf.Sandstone,
            ThingDefOfLocal.Marble,
            ThingDefOfLocal.Slate,
            ThingDefOfLocal.Limestone
        };

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            if (map.AllCells.Where(cell => (!cell.Roofed(map) || (cell.Roofed(map) && cell.GetRoof(map) == RoofDefOf.RoofConstructed)) && cell.DistanceToEdge(map) > 10).TryRandomElement(out IntVec3 pos))
            {
                var faller = SkyfallerPlusMaker.SpawnSkyfaller(ThingDefOfLocal.MeteoriteIncomingPlus, pos, map, () => MeteorImpact(pos, map));

                SendStandardLetter(new LookTargets(pos, map));

                return true;
            }
            else
            {
                return false;
            }
        }

        public void MeteorImpact(IntVec3 pos, Map map)
        {
            CellRect centerRect = CellRect.CenteredOn(pos, 2);
            ThingDef innerItem = things.RandomElement();

            foreach (var item in centerRect)
            {
                if(Rand.Chance(0.7f))
                    GenSpawn.Spawn(innerItem, item, map);
                else
                    GenSpawn.Spawn(ChooseThingDef(), item, map);
            }

            int smallSpot = Rand.Range(1, 4);
            for(int i = 0; i < smallSpot; i++)
            {
                if(CellFinder.TryFindRandomCellNear(centerRect.CenterCell, map, 3, null, out IntVec3 spot))
                {
                    CellRect rect = CellRect.CenteredOn(spot, Rand.Range(2, 3));
                    foreach(var spotPos in rect)
                    {
                        if(Rand.Chance(0.6f))
                            GenSpawn.Spawn(innerItem, spotPos, map);
                        else
                            GenSpawn.Spawn(ChooseThingDef(), spotPos, map);
                    }
                }
            }
        }

        private ThingDef ChooseThingDef()
        {
            return DefDatabase<ThingDef>.AllDefs.Where(def => def.building != null && def.building.isResourceRock).RandomElement();
        }
    }
}
