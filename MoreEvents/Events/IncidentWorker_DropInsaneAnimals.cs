using MoreEvents.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_DropInsaneAnimals : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["DropAnimalInsanity"];

        private List<Thing> animals;
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            if (!ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(parms.points, map.Tile, out PawnKindDef animalKind))
            {
                return false;
            }

            if(!FindHomeAreaCell(map, out IntVec3 result))
            {
                return false;
            }

            parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);

            animals = GetAnimals(animalKind, map, parms.points);
            DropPodUtilityPlus.DropThingsNear(result, map, animals, callback: DriveInsane);
            SendStandardLetter();

            return true;
        }

        private void DriveInsane()
        {
            foreach (var animal in animals)
            {
                Pawn pawn = (Pawn)animal;
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
            }
        }

        private List<Thing> GetAnimals(PawnKindDef animalKind, Map map, float points)
        {
            List<Thing> animals = new List<Thing>();

            List<Pawn> pawns = ManhunterPackIncidentUtility.GenerateAnimals(animalKind, map.Tile, points * 1f);
            foreach (var pawn in pawns)
            {
                animals.Add(pawn);
            }

            return animals;
        }

        private bool FindHomeAreaCell(Map map, out IntVec3 result)
        {
            List<IntVec3> cells = (from c in map.areaManager.Home.ActiveCells where c.GetRoof(map) != RoofDefOf.RoofRockThick && !c.Fogged(map) && c.GetFirstBuilding(map) == null select c).ToList();
            if (cells.Count > 0)
            {
                result = cells.RandomElement();
                return true;
            }

            result = IntVec3.Invalid;
            return false;
        }
    }
}
