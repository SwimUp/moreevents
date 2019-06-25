using MoreEvents.Constellations;
using MoreEvents.Constellations.Actions;
using MoreEvents.Constellations.Conditions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents
{
    public enum ConstellationType : byte
    {
        Positive,
        Negative
    }

    public class ConstellationsDef : Def
    {
        public List<ConstellationCondition> Conditions = new List<ConstellationCondition>();

        public List<ConstellationAction> Actions = new List<ConstellationAction>();

        public List<HediffDef> Effects = new List<HediffDef>();

        public List<InspirationDef> Inspirations = new List<InspirationDef>();

        public bool AllowMale = true;
        public bool AllowFemale = true;

        public ConstellationType ConstellationType = ConstellationType.Positive;

        public static ConstellationsDef Named(string defName)
        {
            return DefDatabase<ConstellationsDef>.GetNamed(defName, true);
        }

    }
}
