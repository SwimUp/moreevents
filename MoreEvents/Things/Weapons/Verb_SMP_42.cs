using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Weapons
{
    public class Verb_SMP_42 : Verb_Shoot
    {
        protected override bool TryCastShot()
        {
            bool flag = false;

            for (int i = 0; i < 5; i++)
                if (base.TryCastShot())
                    flag = true;

            if (flag && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
