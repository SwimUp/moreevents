using MoreEvents.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimOverhaul.AI
{
    public class LordToilData_DefendPawn : LordToilData
    {
        public Pawn defendPawn = null;

        public float defendRadius = 40f;

        public override void ExposeData()
        {
            Scribe_References.Look(ref defendPawn, "defendPawn");
            Scribe_Values.Look(ref defendRadius, "defendRadius", 40f);
        }
    }

    public class LordToil_DefendPawn : LordToil
    {
        private bool allowSatisfyLongNeeds = true;

        protected LordToilData_DefendPawn Data => (LordToilData_DefendPawn)data;

        public Pawn PawnLoc => Data.defendPawn;

        public override bool AllowSatisfyLongNeeds => allowSatisfyLongNeeds;

        public LordToil_DefendPawn(bool canSatisfyLongNeeds = true)
        {
            allowSatisfyLongNeeds = canSatisfyLongNeeds;
            data = new LordToilData_DefendPawn();
        }

        public LordToil_DefendPawn(Pawn defendPawn, float defendRadius = 40f)
            : this()
        {
            Data.defendPawn = defendPawn;
            Data.defendRadius = defendRadius;
        }

        public override void UpdateAllDuties()
        {
            LordToilData_DefendPawn data = Data;
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                lord.ownedPawns[i].mindState.duty = new PawnDuty(DutyDefOfLocal.DefendPawn, data.defendPawn);
                lord.ownedPawns[i].mindState.duty.radius = data.defendRadius;
            }
        }
    }
}
