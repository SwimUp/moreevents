using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Constellations
{
    public class GameCondition_Contellations : GameCondition
    {
        public ConstellationsDef ConstellationsDef;

        private List<BodyPartDef> parts = new List<BodyPartDef>()
        {
            BodyPartDefOf.Head
        };

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref ConstellationsDef, "ConstellationsDef");
        }

        public override void Init()
        {
            base.Init();

            def.label = ConstellationsDef.label;
            def.description = ConstellationsDef.description;

            GiveEffects();

            GiveHeddifs();

            GiveInspirations();
        }

        private void GiveInspirations()
        {
            foreach (var inspir in ConstellationsDef.Inspirations)
            {
                foreach (var map in AffectedMaps)
                {
                    foreach (var pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        pawn.mindState.inspirationHandler.TryStartInspiration(inspir);
                    }
                }
            }
        }

        private void GiveHeddifs()
        {
            foreach (var effect in ConstellationsDef.Effects)
            {
                foreach (var map in AffectedMaps)
                {
                    foreach (var pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(effect, false);
                        if (firstHediffOfDef == null)
                        {
                            HediffGiverUtility.TryApply(pawn, effect, parts);
                        }
                    }
                }
            }
        }

        private void GiveEffects()
        {
            foreach (var action in ConstellationsDef.Actions)
            {
                action.GiveEffect();
            }
        }

        public override void End()
        {
            base.End();

            foreach (var action in ConstellationsDef.Actions)
            {
                action.RemoveEffects();
            }

            foreach (var effect in ConstellationsDef.Effects)
            {
                foreach (var map in AffectedMaps)
                {
                    foreach (var pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(effect, false);
                        if (firstHediffOfDef != null)
                        {
                            pawn.health.RemoveHediff(firstHediffOfDef);
                        }
                    }
                }
            }
        }
    }
}
