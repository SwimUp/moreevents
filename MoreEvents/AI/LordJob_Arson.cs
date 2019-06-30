using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.AI
{
    public class LordJob_Arson : LordJob
    {
        public IntVec3 FireSpot;
        public Faction Faction;

        private Trigger_TicksPassed timeoutTrigger;

        public LordJob_Arson(IntVec3 spot, Faction faction)
        {
            FireSpot = spot;
            Faction = faction;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_Arson fireToil = new LordToil_Arson(FireSpot);
            stateGraph.StartingToil = fireToil;
            LordToil_End lordToil_End = new LordToil_End();
            stateGraph.AddToil(lordToil_End);

            LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_AssaultColony(Faction).CreateGraph()).StartingToil;
            Transition transition2 = new Transition(fireToil, startingToil2);
            transition2.AddTrigger(new Trigger_FractionPawnsLost(0.1f));
            stateGraph.AddTransition(transition2);

            timeoutTrigger = new Trigger_TicksPassed(Rand.RangeInclusive(25000, 50000));
            Transition transition3 = new Transition(fireToil, lordToil_End);
            transition3.AddTrigger(timeoutTrigger);
            transition3.AddPreAction(new TransitionAction_Message("ArsonOut".Translate(), MessageTypeDefOf.SituationResolved));
            stateGraph.AddTransition(transition3);

            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref FireSpot, "FireSpot");
            Scribe_References.Look(ref Faction, "Faction");
        }
    }
}
