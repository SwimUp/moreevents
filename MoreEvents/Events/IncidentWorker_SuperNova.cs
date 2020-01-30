using RimWorld;
using System.Collections.Generic;
using System;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_SuperNova : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Supernova"];

        private float[] _eventChance = new float[4]
        {
            0.35f,
            0.20f,
            0.08f,
            0.03f
        };
        private Action<IncidentParms>[] _events = new Action<IncidentParms>[4]
        {
            new Action<IncidentParms>(x => SupernovaLow(x)),
            new Action<IncidentParms>(x => SupernovaMedium(x)),
            new Action<IncidentParms>(x => SupernovaHigh(x)),
            new Action<IncidentParms>(x => SupernovaUltra(x))
        };

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            while(true)
            {
                int num = Rand.Range(0, 4);

                if(Rand.Chance(_eventChance[num]))
                {
                    _events[num].Invoke(parms);
                    break;
                }
            }

            return true;
        }
        private static void SupernovaLow(IncidentParms parms)
        {
            foreach (var map in Find.Maps)
            {
                IEnumerable<Pawn> pawns = map.mapPawns.FreeColonists;

                foreach (Pawn p in pawns)
                {
                    p.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.AwesomeSight, p);
                }
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_low"), LetterDefOf.NeutralEvent);
        }
        private static void SupernovaMedium(IncidentParms parms)
        {
            GameConditionDef[] conditions = new GameConditionDef[2]
            {
                GameConditionDefOf.HeatWave,
                GameConditionDefOfLocal.BoulderMassHit
            };
            int[] durations = new int[2]
            {
                Rand.Range(10000, 50000),
                Rand.Range(15000, 50000)
            };

            for(int i = 0; i < conditions.Length; i++)
            {
                parms.target.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }

            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_medium"), LetterDefOf.NeutralEvent);
        }
        private static void SupernovaHigh(IncidentParms parms)
        {
            GameConditionDef[] conditions = new GameConditionDef[4]
            {
                GameConditionDefOfLocal.BoulderMassHit,
                GameConditionDefOfLocal.SuperHeatWave,
                GameConditionDefOfLocal.Endlessday,
                GameConditionDefOfLocal.IonizedAtmosphere
            };

            int[] durations = new int[4]
            {
                Rand.Range(70000, 90000),
                Rand.Range(70000, 90000),
                Rand.Range(140000, 170000),
                Rand.Range(70000, 90000)
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                parms.target.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_high"), LetterDefOf.NegativeEvent);
        }
        private static void SupernovaUltra(IncidentParms parms)
        {
            GameConditionDef[] conditions = new GameConditionDef[5]
            {
                GameConditionDefOfLocal.BoulderMassHit,
                GameConditionDefOfLocal.SuperHeatWave,
                GameConditionDefOfLocal.Endlessday,
                GameConditionDefOfLocal.LeanAtmosphere,
                GameConditionDefOfLocal.IonizedAtmosphere
            };

            int[] durations = new int[5]
            {
                Rand.Range(250000, 350000),
                Rand.Range(250000, 350000),
                Rand.Range(250000, 350000),
                -1,
                Rand.Range(250000, 350000)
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                parms.target.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_ultra"), LetterDefOf.NegativeEvent);
        }
    }
}
