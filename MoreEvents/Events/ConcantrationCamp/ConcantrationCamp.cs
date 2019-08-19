using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.Events.ConcantrationCamp
{
    public class ConcantrationCamp : VisitableSite
    {
        public int Timer = 0;
        public List<Pawn> Pawns;

        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapDef => MapDefOfLocal.ConcantrationCamp;

        public override bool CanLeave()
        {
            if (AnyHostileOnMap(Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        private bool AnyHostileOnMap(Map map, Faction enemyFaction)
        {
            List<Pawn> enemyPawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == enemyFaction && p.health.State == PawnHealthState.Mobile && p.RaceProps.Humanlike).ToList();

            if (enemyPawns == null || enemyPawns.Count == 0)
                return false;

            return true;
        }

        public override void Notify_CaravanFormed(Caravan caravan)
        {
            QuestsManager.Communications.CommunicationDialogs.RemoveAll(x => x.RelatedIncident == IncidentDefOfLocal.ConcantrationCamp);

            base.Notify_CaravanFormed(caravan);
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(Faction, new IntVec3(56, 0, 55));
            Lord lord = LordMaker.MakeNewLord(Faction, lordJob_DefendBase, Map);

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.ConcantrationCamp, Map, out List<Pawn> pawns, true, true, true, false, true, true, true, Faction, lord);

            GeneratePrisoners();

            Find.LetterStack.ReceiveLetter("ConcantrationCampTitle".Translate(), "ConcantrationCamp".Translate(), LetterDefOf.NeutralEvent);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToEmptyMap caravanArrivalAction = new CaravanArrivalAction_EnterToEmptyMap(this, new IntVec3(MapDef.size.x, 1, MapDef.size.z));
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanArrivalAction.CanVisit(caravan, mapParent), () => caravanArrivalAction, "EnterMap".Translate(mapParent.LabelCap), caravan, mapParent.Tile, mapParent);
        }

        public void GeneratePawns(int count)
        {
            Pawns = new List<Pawn>();

            for(int i = 0; i < count; i++)
            {
                Pawn p = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist);
                Find.WorldPawns.PassToWorld(p);

                Pawns.Add(p);
            }
        }

        private void GeneratePrisoners()
        {
            Room room = GridsUtility.GetRoom(new IntVec3(65, 0, 80), Map);

            if (Pawns != null)
            {
                foreach (var p in Pawns)
                {
                    Thing t = GenSpawn.Spawn(p, room.Cells.RandomElement(), Map);
                    t.SetFaction(Faction.OfPlayer);

                    if(Find.WorldPawns.Contains(p))
                        Find.WorldPawns.RemovePawn(p);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if(!HasMap)
            {
                Timer--;

                if(Timer <= 0)
                {
                    PrisonersKilled();
                }
            }
        }

        public override void Notify_MyMapRemoved(Map map)
        {
            base.Notify_MyMapRemoved(map);

            if(Pawns != null)
            {
                for(int i = 0; i < Pawns.Count; i++)
                {
                    Pawn pawn = Pawns[i];
                    if(pawn != null)
                    {
                        if (!Find.WorldPawns.Contains(pawn))
                            Find.WorldPawns.PassToWorld(pawn);
                    }
                }
            }
        }

        private void PrisonersKilled()
        {
            if (Pawns != null)
            {
                for (int i = 0; i < Pawns.Count; i++)
                {
                    Pawn pawn = Pawns[i];
                    if (pawn != null)
                    {
                        if (Find.WorldPawns.Contains(pawn))
                            Find.WorldPawns.RemovePawn(pawn);

                        pawn.Destroy();
                    }
                }
            }

            Find.WorldObjects.Remove(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Timer, "Ticks");
            Scribe_Collections.Look(ref Pawns, "Pawns", LookMode.Reference);
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (Pawns != null)
            {
                if (!Map.mapPawns.FreeColonists.Where(x => !Pawns.Contains(x) && !x.Dead && !x.Downed).Any())
                {
                    alsoRemoveWorldObject = false;
                    return true;
                }
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public override string GetInspectString()
        {
            return "InspectString_Timer".Translate(Timer.TicksToDays().ToString("f2"));
        }
    }
}
