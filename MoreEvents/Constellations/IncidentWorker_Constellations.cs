using MoreEvents.Constellations.Conditions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Constellations
{
    public class IncidentWorker_Constellations : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Constellations"];
        private int positiveEnable => int.Parse(settings.Parameters["EnablePositive"].Value);
        private int negativeEnable => int.Parse(settings.Parameters["EnableNegative"].Value);

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<ConstellationsDef> toInvoke = new List<ConstellationsDef>();

            foreach (var con in DefDatabase<ConstellationsDef>.AllDefs.Where(c => (c.ConstellationType == ConstellationType.Positive && positiveEnable == 1) || (c.ConstellationType == ConstellationType.Negative && negativeEnable == 1)))
            {
                List<ConstellationCondition> conditions = con.Conditions;

                bool canSpawn = true;
                foreach (var condition in conditions)
                {
                    canSpawn = condition.CanSpawn(null);
                    if(!canSpawn)
                        break;
                }

                if(canSpawn)
                {
                    toInvoke.Add(con);
                }
            }

            if(toInvoke.Count > 0)
            {
                ConstellationsDef constellationsDef = toInvoke.RandomElement();
                GameCondition_Contellations condition = (GameCondition_Contellations)GameConditionMaker.MakeCondition(GameConditionDefOfLocal.Constellations, 5 * 60000);
                condition.ConstellationsDef = constellationsDef;
                Find.World.gameConditionManager.RegisterCondition(condition);

                SendLetter(constellationsDef);

                return true;
            }

            return false;
        }

        private void SendLetter(ConstellationsDef def)
        {
            Find.LetterStack.ReceiveLetter(def.label, def.description, LetterDefOf.NeutralEvent);
        }
    }
}
