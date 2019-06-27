using MapGeneratorBlueprints.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class DoomsdaySite : VisitableSite
    {
        public static DoomsdaySite ActiveSite;

        public bool WeaponDeactivated = false;

        public CaravanArrivalAction_Doomsday caravanAction;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RemoveAfterLeave = true;

            caravanAction = new CaravanArrivalAction_Doomsday(this);
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            foreach(var cell in Map.AllCells)
            {
                Map.terrainGrid.SetTerrain(cell, TerrainDefOf.Soil);
            }

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.Doomsday, Map, true, true, true, false, true, true, true, Faction);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref WeaponDeactivated, "WeaponDeactivated");
            Scribe_References.Look(ref ActiveSite, "ActiveSite");
        }

        public override bool CanLeave()
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(this.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (!WeaponDeactivated)
            {
                Messages.Message(Translator.Translate("WeaponDeactivated"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }
    }
}
