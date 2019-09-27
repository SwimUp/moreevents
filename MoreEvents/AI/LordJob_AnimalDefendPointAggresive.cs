using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.AI
{
    public class LordJob_AnimalDefendPointAggresive : LordJob
    {
        private Faction faction;

        private IntVec3 baseCenter;

        public LordJob_AnimalDefendPointAggresive()
        {
        }
        public LordJob_AnimalDefendPointAggresive(Faction faction, IntVec3 baseCenter)
        {
            this.faction = faction;
            this.baseCenter = baseCenter;
        }
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_DefendBase lordToil_DefendBase = (LordToil_DefendBase)(stateGraph.StartingToil = new LordToil_DefendBase(baseCenter));
            LordToil_DefendBase lordToil_DefendBase2 = new LordToil_DefendBase(baseCenter);
            stateGraph.AddToil(lordToil_DefendBase2);
            LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(attackDownedIfStarving: true);
            lordToil_AssaultColony.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_AssaultColony);
            Transition transition = new Transition(lordToil_DefendBase, lordToil_DefendBase2);
            transition.AddSource(lordToil_AssaultColony);
            transition.AddTrigger(new Trigger_BecameNonHostileToPlayer());
            stateGraph.AddTransition(transition);
            Transition transition2 = new Transition(lordToil_DefendBase2, lordToil_DefendBase);
            transition2.AddTrigger(new Trigger_BecamePlayerEnemy());
            stateGraph.AddTransition(transition2);
            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref baseCenter, "baseCenter");
        }
    }
}
