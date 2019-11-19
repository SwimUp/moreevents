using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace RimOverhaul
{
    public static class ReserachUtility
    {
        private static Dictionary<ResearchProjectDef, float> progress;

        public static void AddPoints(ResearchProjectDef researchProjectDef, int points)
        {
            if (progress == null)
            {
                FieldInfo progressField = typeof(ResearchManager).GetField("progress", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Static);

                progress = progressField.GetValue(Find.ResearchManager) as Dictionary<ResearchProjectDef, float>;
            }

            progress[researchProjectDef] += points;
        }
    }
}
