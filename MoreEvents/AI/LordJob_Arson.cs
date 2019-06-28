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

        public LordJob_Arson(IntVec3 spot)
        {
            FireSpot = spot;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_Travel travel = new LordToil_Travel(FireSpot);
            Log.Message($"{FireSpot}");
            stateGraph.AddToil(travel);
            LordToil_Arson fireToil = new LordToil_Arson(FireSpot);
            stateGraph.AddToil(fireToil);

            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref FireSpot, "FireSpot");
        }
    }
}
