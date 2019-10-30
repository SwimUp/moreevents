using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul
{
    [StaticConstructorOnStartup]
    public static class PriceUtils
    {
        public static bool TakeSilverFromPlayer(int count, Map map)
        {
            int playerSilver = map.resourceCounter.Silver;
            if (playerSilver >= count)
            {
                int remaining = count;
                List<Thing> silver = map.listerThings.ThingsOfDef(ThingDefOf.Silver);
                for (int i = 0; i < silver.Count; i++)
                {
                    Thing item = silver[i];

                    int num = Mathf.Min(remaining, item.stackCount);
                    item.SplitOff(num).Destroy();
                    remaining -= num;

                    if (remaining == 0)
                        break;
                }

                map.resourceCounter.UpdateResourceCounts();

                return true;
            }
            else
            {
                Messages.Message("CommOption_NonAgressionPact_NotEnoughSilver".Translate(count, playerSilver), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }
        }
    }
}
