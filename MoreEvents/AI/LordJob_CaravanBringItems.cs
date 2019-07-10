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
        private Pawn gifter;

        public LordJob_CaravanBringItems()
        {

        }

        public LordJob_CaravanBringItems(IntVec3 spot, Pawn gifter)
        {
            this.spot = spot;
            this.gifter = gifter;
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
                Thing targetItem = gifter.inventory.innerContainer.RandomElement();
                gifter.inventory.innerContainer.TryDropAll(gifter.Position, gifter.Map, ThingPlaceMode.Near);
                Find.LetterStack.ReceiveLetter("GiftHasBeenDroppedTitle".Translate(), "GiftHasBeenDropped".Translate(), LetterDefOf.PositiveEvent, new LookTargets(targetItem));
            }));
            stateGraph.AddTransition(transition);

            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref gifter, "gifter");
            Scribe_Values.Look(ref spot, "spot");
        }
    }
}
