using MapGeneratorBlueprints.MapGenerator;
using MoreEvents.Things;
using QuestRim;
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

        public Building_DoomsdayGun Weapon;

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

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                var letter = LetterMaker.MakeLetter("YourAttackWasRepelledTitle".Translate(), "YourAttackWasRepelledDesc".Translate(), LetterDefOf.NeutralEvent);
                Find.LetterStack.ReceiveLetter(letter);

                alsoRemoveWorldObject = false;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            foreach (var cell in Map.AllCells)
            {
                Map.terrainGrid.SetTerrain(cell, TerrainDefOf.Soil);
            }

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.Doomsday, Map, out List<Pawn> pawns, true, true, true, false, true, true, true, Faction);

            IntVec3 spawnPos = new IntVec3(126, 0, 163);
            Thing thing = ThingMaker.MakeThing(ThingDefOfLocal.DoomsdayUltimateBomb);
            thing.SetFaction(Faction);
            Weapon = (Building_DoomsdayGun)GenSpawn.Spawn(thing, spawnPos, Map);

            ShowHelp();
        }

        private void ShowHelp()
        {
            DiaNode node = new DiaNode("DoomsdayEnterSiteHelp".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref Weapon, "Weapon");
            Scribe_References.Look(ref ActiveSite, "ActiveSite");
        }

        public override bool CanLeave()
        {
            if (AnyHostileOnMap(this.Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (!Weapon.WeaponDeactivated)
            {
                Messages.Message(Translator.Translate("WeaponDeactivated"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        private bool AnyHostileOnMap(Map map, Faction enemyFaction)
        {
            List<Pawn> enemyPawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == enemyFaction && !p.Dead && p.RaceProps.Humanlike).ToList();

            if (enemyPawns == null || enemyPawns.Count == 0)
                return false;

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
                for(int i = 0; i < comp.CachedRelations.Count; i++)
                {
                    FactionRelation rel = comp.CachedRelations[i];
                    Faction fact = comp.CachedFactions[i];
                    fact.TrySetRelationKind(rel.other, rel.kind);
                }

                comp.CachedRelations.Clear();
                comp.CachedFactions.Clear();
            }

            QuestsManager.Communications.RemoveCommunication(comp.Dialog);
            comp.Dialog = null;

            base.PreForceReform(mapParent);
        }
    }
}
