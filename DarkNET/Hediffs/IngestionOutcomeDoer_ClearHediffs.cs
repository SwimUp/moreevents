using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class IngestionOutcomeDoer_ClearHediffs : IngestionOutcomeDoer
    {
        public List<HediffDef> Hediffs;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if(Hediffs != null)
            {
                foreach(var hediffDef in Hediffs)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if(hediff != null)
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}
