using MoreEvents.Weather;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_Psychogas : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["Psychogas"];

        private SkyColorSet PsychoGasColors = new SkyColorSet(new ColorInt(82, 82, 82).ToColor, new ColorInt(153, 147, 147).ToColor, new ColorInt(0, 72, 255).ToColor, 0.95f);

        private List<SkyOverlay> overlays = new List<SkyOverlay>
        {
            new WeatherOverlay_PsychoGas()
        };

        public override void Init()
        {
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Critical);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(0.55f, PsychoGasColors, 1f, 1f);
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return overlays;
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % 3500 == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    DoJoyEffect(affectedMaps[i]);
                }
            }
            for (int j = 0; j < overlays.Count; j++)
            {
                for (int k = 0; k < affectedMaps.Count; k++)
                {
                    overlays[j].TickOverlay(affectedMaps[k]);
                }
            }
        }

        private void DoJoyEffect(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (CanDamage(pawn, map))
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.MentalImpact);
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].DrawOverlay(map);
            }
        }

        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }

        public bool CanDamage(Pawn pawn, Map map)
        {
            if (pawn.Dead || pawn.Position.Roofed(map) || !pawn.RaceProps.Humanlike)
            {
                return false;
            }

            return true;
        }
    }
}
