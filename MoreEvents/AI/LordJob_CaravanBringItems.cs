using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MoreEvents.AI
{
    public class LordJob_CaravanBringItems : LordJob
    {
        private IntVec3 spot;
        private List<Pawn> gifters = new List<Pawn>();

        public LordJob_CaravanBringItems()
        {

        }

        public LordJob_CaravanBringItems(IntVec3 spot, Pawn gifter)
        {
            this.spot = spot;
            gifters.Add(gifter); 
        }

        public LordJob_CaravanBringItems(IntVec3 spot, List<Pawn> gifters)
        {
            this.spot = spot;
            this.gifters = gifters;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil lordToil = stateGraph.StartingToil = stateGraph.AttachSubgraph(new LordJob_Travel(spot).CreateGraph()).StartingToil;

            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Walk, canDig: true);
            stateGraph.AddToil(lordToil_ExitMap);

            Transition transition = new Transition(lordToil, lordToil_ExitMap);
            transition.AddTrigger(new Trigger_Memo("TravelArrived"));
            transition.AddPreAction(new TransitionAction_Custom(delegate (Transition t)
            {
                IntVec3 seekPos = gifters.First().Position;
                foreach (var p in gifters)
                    p.inventory.innerContainer.TryDropAll(p.Position, p.Map, ThingPlaceMode.Near);

                Find.LetterStack.ReceiveLetter("GiftHasBeenDroppedTitle".Translate(), "GiftHasBeenDropped".Translate(), LetterDefOf.PositiveEvent, new LookTargets(seekPos, Map));
            }));
            stateGraph.AddTransition(transition);

            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref gifters, "gifters");
            Scribe_Values.Look(ref spot, "spot");
        }
    }
}
