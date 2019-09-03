﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    [StaticConstructorOnStartup]
    public static class DarkNetPriceUtils
    {
        public static ThingFilter GetThingFilter(List<DarkNetGood> goods)
        {
            ThingFilter filter = new ThingFilter();

            foreach(var good in goods)
            {
                if (Rand.Chance(good.Commonality))
                {
                    foreach (var thing in good.ThingFilter.AllowedThingDefs)
                    {
                        filter.SetAllow(thing, true);
                    }
                }
            }

            return filter;
        }

        public static void FinalizeItem(Thing item, PriceModificatorDef priceModificatorDef)
        {
            MinifiedThing minifiedThing = item as MinifiedThing;
            if (minifiedThing != null)
            {
                item = minifiedThing.InnerThing;
            }

            var comp = item.TryGetComp<CompQuality>();
            if (comp != null)
            {
                if (comp.Quality > priceModificatorDef.QualityRange.max)
                    comp.SetQuality(priceModificatorDef.QualityRange.max, ArtGenerationContext.Colony);
                else if(comp.Quality < priceModificatorDef.QualityRange.min)
                    comp.SetQuality(priceModificatorDef.QualityRange.min, ArtGenerationContext.Colony);
            }

            if (priceModificatorDef.HealthRange.max != 0)
            {
                int newHp = (int)priceModificatorDef.HealthRange.RandomInRange;
                item.HitPoints = newHp;
            }
        }

        public static bool BuyAndDropItem(SellableItemWithModif tradeItem, Map map, bool receiveLetter = true)
        {
            int playerSilver = map.resourceCounter.Silver;
            if (playerSilver >= tradeItem.MarketValue)
            {
                int remaining = tradeItem.MarketValue;
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

                List<Thing> toTrade = new List<Thing>
                {
                    tradeItem.Item
                };

                IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
                DropPodUtility.DropThingsNear(intVec, map, toTrade, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

                if (receiveLetter)
                    Find.LetterStack.ReceiveLetter("BuyAndDropItem_NotifyTitle".Translate(), "BuyAndDropItem_NotifyDesc".Translate(), LetterDefOf.PositiveEvent, toTrade);

                tradeItem.Item = null;

                map.resourceCounter.UpdateResourceCounts();

                return true;
            }
            else
            {
                Messages.Message("CommOption_NonAgressionPact_NotEnoughSilver".Translate(tradeItem.MarketValue, playerSilver), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }
        }

        public static bool BuyAndDropItem(Thing tradeItem, int price, Map map, bool receiveLetter = true)
        {
            int playerSilver = map.resourceCounter.Silver;
            if (playerSilver >= price)
            {
                int remaining = price;
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

                List<Thing> toTrade = new List<Thing>
                {
                    tradeItem
                };

                IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
                DropPodUtility.DropThingsNear(intVec, map, toTrade, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

                if(receiveLetter)
                    Find.LetterStack.ReceiveLetter("BuyAndDropItem_NotifyTitle".Translate(), "BuyAndDropItem_NotifyDesc".Translate(), LetterDefOf.PositiveEvent, toTrade);

                map.resourceCounter.UpdateResourceCounts();

                return true;
            }
            else
            {
                Messages.Message("CommOption_NonAgressionPact_NotEnoughSilver".Translate(tradeItem.MarketValue, playerSilver), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }
        }

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
