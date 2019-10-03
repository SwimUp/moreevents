using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimOverhaul.Things.Special
{
    public class SphereOfResurrection : UsableItem
    {
        public override void Use()
        {
            List<Thing> corpses = Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).ToList();
            for(int i = 0; i < corpses.Count; i++)
            {
                Thing corpseThing = corpses[i];

                Corpse corpse = corpseThing as Corpse;
                if(corpse != null)
                {
                    Pawn pawn = corpse.InnerPawn;
                    if(pawn != null && pawn.RaceProps.IsFlesh)
                    {
                        ResurrectionUtility.ResurrectWithSideEffects(pawn);

                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ResurrectionSickness);
                        if (hediff != null)
                            pawn.health.RemoveHediff(hediff);

                        HealthUtility.AdjustSeverity(pawn, HediffDefOfLocal.SphereOfResurrection, 0.1f);
                    }
                }
            }

            Destroy();
        }
    }
}
