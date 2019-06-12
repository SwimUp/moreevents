using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_Earthquake : GameCondition
    {
        private int magnitude = 0;

        private readonly int[] tremorsCount = new int[]
        {
            7,
            13,
            17,
            22,
            26,
            29,
            33,
            35,
            45,
            50
        };

        private readonly float[] damageMultipliers = new float[]
        {
            0.5f,
            0.9f,
            1.2f,
            1.3f,
            1.5f,
            1.6f,
            1.8f,
            2.0f,
            2.2f,
            2.5f
        };

        private float damageMultiplier = 1.0f;

        private int cycleTimer = 0;
        private int cycle => 400;
        private int tremors = 0;

        private EventSettings settings => Settings.EventsSettings["Earthquake"];

        public override void Init()
        {
            if(!settings.Active)
            {
                End();
            }

            base.Init();

            magnitude = Rand.Range(0, 9);
            tremors = tremorsCount[magnitude];
            cycleTimer = cycle;
            damageMultiplier = damageMultipliers[magnitude];
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if(tremors == 0)
            {
                Find.LetterStack.ReceiveLetter(Translator.Translate("Earthshake"), $"{Translator.Translate("EarthshakeDesc")} {magnitude}", LetterDefOf.PositiveEvent);
                End();
            }

            cycleTimer--;

            if (cycleTimer <= 0)
            {
                cycleTimer = cycle;
                tremors--;

                DoShake();
            }
        }

        public override void End()
        {
            base.End();
        }

        private void DoShake()
        {
            Find.CameraDriver.shaker.DoShake(magnitude / 2);
            int count = Rand.Range(7, 17);

            for (int i = 0; i < count; i++)
            {
                var thing = SingleMap.spawnedThings.Where(t => t is Building && t.Position.GetRoof(SingleMap) == RoofDefOf.RoofConstructed && !t.Fogged()).RandomElement();
                float damage = Rand.Range(10, 20) * damageMultiplier;
                thing.TakeDamage(new DamageInfo(DamageDefOf.Bomb, damage));
            }
        }

    }
}
