using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.SiegeCamp
{
    public class SiegeCampSite : VisitableSite
    {
        public CaravanVisitAction_SiegeCamp caravanAction;

        public CommunicationDialog Dialog;

        private SiegeCampSiteComp comp;

        public Map PlayerSiegeMap
        {
            get
            {
                if(playerSiegeMap == null)
                {
                    playerSiegeMap = Current.Game.FindMap(MapSiegeTile);
                }

                return playerSiegeMap;
            }
        }
        private Map playerSiegeMap = null;
        public int MapSiegeTile = -1;

        private bool victory = false;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RemoveAfterLeave = false;

            comp = this.GetComponent<SiegeCampSiteComp>();

            caravanAction = new CaravanVisitAction_SiegeCamp(this);
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                if (!victory)
                {
                    comp.UpdateCamp();

                    comp.Start();
                }

                alsoRemoveWorldObject = false;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public void SetMap(Map map)
        {
            playerSiegeMap = map;
            MapSiegeTile = playerSiegeMap.Tile;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref MapSiegeTile, "MapSiegeTile");
            Scribe_References.Look(ref Dialog, "Dialog");
        }

        public override void PostMapGenerate()
        {
            comp.Stop();

            base.PostMapGenerate();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        public override bool CanLeave()
        {
            if (AnyHostileOnMap(this.Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
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

        public override void PreForceReform(MapParent mapParent)
        {
            comp.Start();

            victory = true;

            RemoveAfterLeave = true;

            QuestsManager.Communications.RemoveCommunication(Dialog);

            base.PreForceReform(mapParent);
        }
    }
}
