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
        public CaravanArrivalAction_GiveRansom caravanAction2;

        public DoomsdayUltimatumComp comp;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RemoveAfterLeave = true;

            caravanAction = new CaravanArrivalAction_Doomsday(this);
            caravanAction2 = new CaravanArrivalAction_GiveRansom(this);

            comp = GetComponent<DoomsdayUltimatumComp>();
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
            foreach(var option in CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent))
            {
                yield return option;
            }

            int reqCount = 50000 - comp.FactionSilver;
            bool hasSilver = CaravanInventoryUtility.HasThings(caravan, ThingDefOf.Silver, reqCount);
            foreach (var option in CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction2.CanGiveRansom(caravan, mapParent), () => caravanAction2, hasSilver ? "GiveRansom".Translate() : "NotEnoughSilverDoom".Translate(), caravan, mapParent.Tile, mapParent))
            {
                yield return option;
            }
        }

        public override void PreForceReform(MapParent mapParent)
        {
            if (comp.CachedRelations != null)
            {
                foreach (var cache in comp.CachedRelations)
                {
                    foreach(var relation in cache.Value)
                    {
                        cache.Key.TrySetRelationKind(relation.other, relation.kind);
                    }
                }

                comp.CachedRelations.Clear();
            }

            base.PreForceReform(mapParent);
        }
    }
}
