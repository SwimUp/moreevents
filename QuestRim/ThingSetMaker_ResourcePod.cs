using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class ThingSetMaker_ResourcePod : ThingSetMaker
    {
        private int MaxStacks = 7;

        private float MaxMarketValue = 40f;

        private float MinMoney = 150f;

        private float MaxMoney = 600f;

        private int minStack;
        private int maxStack;

        public ThingSetMaker_ResourcePod(int maxStacks, float maxMarketValue, float minMoney, float maxMoney, int minStack = 20, int maxStack = 60)
        {
            MaxStacks = maxStacks;
            MaxMarketValue = maxMarketValue;
            MinMoney = minMoney;
            MaxMoney = maxMoney;

            this.minStack = minStack;
            this.maxStack = maxStack;
        }

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            ThingDef thingDef = RandomPodContentsDef();
            float num = Rand.Range(MinMoney, MaxMoney);
            do
            {
                Thing thing = ThingMaker.MakeThing(thingDef);
                int num2 = Rand.Range(minStack, maxStack);
                if (num2 > thing.def.stackLimit)
                {
                    num2 = thing.def.stackLimit;
                }
                if ((float)num2 * thing.def.BaseMarketValue > num)
                {
                    num2 = Mathf.FloorToInt(num / thing.def.BaseMarketValue);
                }
                if (num2 == 0)
                {
                    num2 = 1;
                }
                thing.stackCount = num2;
                outThings.Add(thing);
                num -= (float)num2 * thingDef.BaseMarketValue;

                thingDef = RandomPodContentsDef();
            }
            while (outThings.Count < MaxStacks && !(num <= thingDef.BaseMarketValue));
        }

        private IEnumerable<ThingDef> PossiblePodContentsDefs()
        {
            return from d in DefDatabase<ThingDef>.AllDefs
                   where d.category == ThingCategory.Item && d.tradeability.TraderCanSell() && d.equipmentType == EquipmentType.None && d.BaseMarketValue >= 1f && d.BaseMarketValue < MaxMarketValue && !d.HasComp(typeof(CompHatcher))
                   select d;
        }

        private ThingDef RandomPodContentsDef()
        {
            int numMeats = (from x in PossiblePodContentsDefs()
                            where x.IsMeat
                            select x).Count();
            int numLeathers = (from x in PossiblePodContentsDefs()
                               where x.IsLeather
                               select x).Count();
            return PossiblePodContentsDefs().RandomElementByWeight((ThingDef d) => ThingSetMakerUtility.AdjustedBigCategoriesSelectionWeight(d, numMeats, numLeathers));
        }
        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            return PossiblePodContentsDefs();
        }
    }
}
