using MapGeneratorBlueprints.MapGenerator;
using MoreEvents.Events;
using QuestRim;
using RimOverhaul.Quests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events.Fair
{
    public class FairPlace : VisitableSite
    {
        private int ticksToRemove;

        public bool FairEnded = false;

        public CommunicationDialog CommunicationDialog;

        public string MapGeneratorTag => "IncidentFair";

        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapGenerator;

        public override bool ShowButton => false;
        public FairPlace()
        {

        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            CaravanArrivalAction_EnterToMapWithGenerator caravanAction = new CaravanArrivalAction_EnterToMapWithGenerator(this, MapGenerator);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMap_Option".Translate(), caravan, this.Tile, this);
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            MapGeneratorHandler.GenerateMap(MapGenerator, Map, out List<Pawn> pawns, false, true, true, false, true, true, true, Faction);

            int startCaravans = Rand.Range(2, 5);
            for (int i = 0; i < startCaravans; i++)
                SendNewCaravan();
        }

        public override void Tick()
        {
            base.Tick();

            if (!FairEnded)
            {
                ticksToRemove--;
                if (ticksToRemove <= 0)
                {
                    EndFair();
                }
            }

            if (HasMap)
            {
                if (Find.TickManager.TicksGame % 140000 == 0)
                {
                    SendNewCaravan();
                }
            }
        }

        public void SendNewCaravan()
        {
            IncidentDefOf.TraderCaravanArrival.Worker.TryExecute(new IncidentParms
            {
                target = Map
            });
        }

        public override string GetInspectString()
        {
            return $"FairPlace_GetInspectString".Translate(ticksToRemove.TicksToDays().ToString("f2"));
        }

        public void EndFair()
        {
            FairEnded = true;

            Find.LetterStack.ReceiveLetter("Fair_EndTitle".Translate(), "Fair_EndDesc".Translate(), LetterDefOf.PositiveEvent);

            if (CommunicationDialog != null)
            {
                QuestsManager.Communications.RemoveCommunication(CommunicationDialog);
            }

            if (HasMap && Map.mapPawns.AnyColonistSpawned)
            {
                RemoveAfterLeave = true;
                ForceReform(this);
            }
            else
            {
                Find.WorldObjects.Remove(this);
            }
        }
        public void SetTimer(int days)
        {
            ticksToRemove = days * 60000;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticksToRemove, "ticksToRemove");
            Scribe_References.Look(ref CommunicationDialog, "CommunicationDialog");
            Scribe_Values.Look(ref FairEnded, "FairEnded");
            Scribe_Defs.Look(ref MapGenerator, "MapGenerator");
        }
    }
}
