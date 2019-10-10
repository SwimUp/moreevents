using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkNET.Traders;
using RimWorld;
using Verse;

namespace DarkNET.Sly
{
    public class SlyService_ItemsHelp : SlyHelp_OneUse
    {
        public override string Label => "";

        public override string Description => "";

        private int totalUses = 0;
        private int lastHelpTicks;

        private int lastCheckRaids;
        private int lastCheckRaidsInterval;
        private bool tooDangerous;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref totalUses, "totalUses");
            Scribe_Values.Look(ref lastHelpTicks, "lastHelpTicks");
            Scribe_Values.Look(ref lastCheckRaids, "lastCheckRaids");
            Scribe_Values.Look(ref lastCheckRaidsInterval, "lastCheckRaidsInterval");
            Scribe_Values.Look(ref tooDangerous, "tooDangerous");
        }

        public override IEnumerable<FloatMenuOption> Options(Map map)
        {
            yield break;
        }

        protected void GenerateAndSendHelp(int price, Map map, Dictionary<ThingDef, int> items)
        {
            if (!DarkNetPriceUtils.TakeSilverFromPlayer(price, map))
                return;

            List<Thing> toDrop = new List<Thing>();
            foreach(var item in items)
            {
                Thing newItem = ThingMaker.MakeThing(item.Key);
                newItem.stackCount = item.Value;

                toDrop.Add(newItem);
            }

            DropItems(toDrop, map);

            totalUses++;
            alreadyUsed = true;

            if (totalUses >= 3)
                lastHelpTicks = Find.TickManager.TicksGame;
        }

        protected void GenerateAndSendHelp(int price, Map map, FloatRange priceRange, IntRange countRange, List<ThingDef> items)
        {
            if (!DarkNetPriceUtils.TakeSilverFromPlayer(price, map))
                return;

            List<Thing> toDrop = new List<Thing>();
            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();

            ThingSetMakerParams parms = default;
            parms.totalMarketValueRange = priceRange;
            parms.countRange = countRange;

            ThingFilter filter = new ThingFilter();
            foreach(var item in items)
            {
                filter.SetAllow(item, true);
            }

            parms.filter = filter;

            maker.fixedParams = parms;

            toDrop = maker.Generate();

            DropItems(toDrop, map);

            totalUses++;
            alreadyUsed = true;

            if (totalUses >= 3)
                lastHelpTicks = Find.TickManager.TicksGame;
        }

        private void DropItems(List<Thing> items, Map map)
        {
            IntVec3 dropCell = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.DropThingsNear(dropCell, map, items, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

            Letter letter = LetterGetter();
            letter.lookTargets = new LookTargets(dropCell, map);

            Find.LetterStack.ReceiveLetter(LetterGetter());
        }

        public virtual Letter LetterGetter()
        {
            return LetterMaker.MakeLetter("SlyService_ItemsHelp_Letter_Title".Translate(), "SlyService_ItemsHelp_Letter_Description".Translate(), LetterDefOf.PositiveEvent);
        }

        public override bool AvaliableRightNow(out string reason)
        {
            if(totalUses >= 3)
            {
                reason = "SlyService_HumanitarianHelp_TooManyUses".Translate();
                return false;
            }

            if(tooDangerous)
            {
                reason = "SlyService_HumanitarianHelp_TooDangerous".Translate();
                return false;
            }

            return base.AvaliableRightNow(out reason);
        }

        public override void SlyDayPassed(TraderWorker_Sly sly)
        {
            base.SlyDayPassed(sly);

            if (totalUses >= 3)
            {
                int passedDays = (Find.TickManager.TicksGame - lastHelpTicks) / 60000;
                if (passedDays >= 15)
                {
                    totalUses = 0;
                }
            }

            int checkInterval = (Find.TickManager.TicksGame - lastCheckRaidsInterval) / 60000;
            if (checkInterval >= 3)
            {
                lastCheckRaidsInterval = Find.TickManager.TicksGame;

                int raidsNum = Find.StoryWatcher.statsRecord.numRaidsEnemy - lastCheckRaids;
                tooDangerous = raidsNum >= 3 ? true : false;

                lastCheckRaids = Find.StoryWatcher.statsRecord.numRaidsEnemy;
            }
        }
    }
}
