using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.AI
{
    public class LordJob_DefendPawn : LordJob
    {
        private Pawn targetPawn;

        public LordJob_DefendPawn()
        {
        }

        public LordJob_DefendPawn(Pawn targetPawn)
        {
            this.targetPawn = targetPawn;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            stateGraph.AddToil(new LordToil_DefendPawn(targetPawn));
            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref targetPawn, "targetPawn");
        }
    }
}
