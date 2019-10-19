using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Thoughts
{
    public class ThoughtWorker_Ibuprofien : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            if (firstHediffOfDef == null || firstHediffOfDef.def.stages == null)
            {
                return ThoughtState.Inactive;
            }

            return ThoughtState.ActiveAtStage(firstHediffOfDef.CurStageIndex);
        }
    }
}
