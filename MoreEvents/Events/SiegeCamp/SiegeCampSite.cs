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

        private int checkColonistTicker = 0;
        private int checkColonistInterval = 5000;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RemoveAfterLeave = false;

            comp = this.GetComponent<SiegeCampSiteComp>();

            caravanAction = new CaravanVisitAction_SiegeCamp(this);
        }

        public override void Tick()
        {
            base.Tick();

            if (HasMap)
            {
                checkColonistTicker--;
                if (checkColonistTicker <= 0)
                {
                    checkColonistTicker = checkColonistInterval;

                    CheckColonistsNow();
                }
            }
        }

        public void CheckColonistsNow()
        {
            List<Pawn> pawns = Map.mapPawns.FreeColonists.ToList();

            int downedPawns = 0;
            pawns.ForEach(delegate (Pawn p)
            {
                if (p.Downed || p.Dead || !p.Spawned)
                {
                    downedPawns++;
                }
            });

            if (downedPawns == pawns.Count)
            {
                comp.UpdateCamp();
            }
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
            if (GenHostility.AnyHostileActiveThreatToPlayer(this.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override void PreForceReform(MapParent mapParent)
        {
            comp.Start();

            RemoveAfterLeave = true;

            QuestsManager.Communications.RemoveCommunication(Dialog);

            base.PreForceReform(mapParent);
        }
    }
}
