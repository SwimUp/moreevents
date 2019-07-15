using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffComp_AutoHeal : HediffComp
    {
        private HediffCompProperties_AutoHeal Props => (HediffCompProperties_AutoHeal)props;

        private int healTick = 0;

        public override void CompPostMake()
        {
            base.CompPostMake();

            healTick = Props.HealTicks;
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            healTick--;

            if(healTick <= 0)
            {
                healTick = Props.HealTicks;

                DoHeal();
            }
        }

        private void DoHeal()
        {
            IEnumerable<Hediff_Injury> injuries = Pawn.health.hediffSet.GetInjuriesTendable();
            if(injuries.TryRandomElement(out Hediff_Injury inj))
            {
                inj.Heal(Props.HealModiff);
            }
        }
    }
}
