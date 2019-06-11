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
        private Action<Map>[] _events = new Action<Map>[4]
        {
            new Action<Map>(x => SupernovaLow(x)),
            new Action<Map>(x =>  SupernovaMedium(x)),
            new Action<Map>(x =>  SupernovaHigh(x)),
            new Action<Map>(x =>  SupernovaUltra(x))
        };

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            while(true)
            {
                int num = Rand.Range(0, 4);

                if(Rand.Chance(_eventChance[num]))
                {
                    _events[num].Invoke(map);
                    break;
                }
            }

            return true;
        }
        private static void SupernovaLow(Map map)
        {
            IEnumerable<Pawn> pawns = map.mapPawns.FreeColonists;

            foreach (Pawn p in pawns)
            {
                p.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.AwesomeSight, p);
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_low"), LetterDefOf.NeutralEvent);
        }
        private static void SupernovaMedium(Map map)
        {
            GameConditionDef[] conditions = new GameConditionDef[2]
            {
                GameConditionDefOf.HeatWave,
                GameConditionDefOfLocal.BoulderMassHit
            };
            int[] durations = new int[2]
            {
                Rand.Range(40000, 150000),
                Rand.Range(120000, 300000)
            };

            for(int i = 0; i < conditions.Length; i++)
            {
                map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }

            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_medium"), LetterDefOf.NeutralEvent);
        }
        private static void SupernovaHigh(Map map)
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
                Rand.Range(500000, 900000),
                Rand.Range(150000, 300000),
                Rand.Range(250000, 800000),
                Rand.Range(200000, 500000)
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_high"), LetterDefOf.NegativeEvent);
        }
        private static void SupernovaUltra(Map map)
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
                Rand.Range(900000, 3000000),
                Rand.Range(400000, 650000),
                Rand.Range(900000, 3000000),
                -1,
                Rand.Range(500000, 900000)
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(conditions[i], durations[i]));
            }
            Find.LetterStack.ReceiveLetter(Translator.Translate("Supernova_label"), Translator.Translate("SupernovaExp_ultra"), LetterDefOf.NegativeEvent);
        }
    }
}
