using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class HealBelt : Apparel
    {
        public int DamagePerHeal => 1;

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(50))
            {
                TryHeal();
            }
        }

        private void TryHeal()
        {
            if (Wearer == null)
                return;

            if (!Wearer.RaceProps.Humanlike || Wearer.Dead)
                return;

            IEnumerable<Hediff_Injury> injuries = Wearer.health.hediffSet.GetInjuriesTendable();
            if (injuries.TryRandomElement(out Hediff_Injury inj))
            {
                inj.Heal(5);
                TakeDamage(new DamageInfo(DamageDefOf.Crush, DamagePerHeal));
            }
        }
    }
}
