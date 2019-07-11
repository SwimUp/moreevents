using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public enum EndCondition
    {
        Timeout,
        Fail,
        Success,
        None
    };

    public abstract class Quest : IExposable, ILoadReferenceable
    {
        public int id;

        public virtual string AdditionalQuestContentString => "AdditionalContent".Translate();

        public abstract string CardLabel { get; }
        public abstract string Description { get; }

        public virtual string ExpandingIconPath { get; }

        public virtual Texture2D ExpandingIcon
        {
            get
            {
                if (expandingIcon == null)
                {
                    if (ExpandingIconPath.NullOrEmpty())
                    {
                        return null;
                    }

                    expandingIcon = ContentFinder<Texture2D>.Get(ExpandingIconPath);
                }

                return expandingIcon;
            }
        }
        private Texture2D expandingIcon = null;

        public virtual string PlaceLabel { get; }

        public Faction Faction;
        public LookTargets Target;

        public List<Thing> Rewards = new List<Thing>();
        public List<QuestOption> Options;

        public int TicksToPass = 60000;
        public bool UnlimitedTime = false;

        public virtual void SiteTick()
        {

        }

        public virtual void GenerateRewards(ThingFilter filter, FloatRange totalValue, IntRange countRange, TechLevel? techLevel, float? totalMass)
        {
            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms2 = default;
            parms2.totalMarketValueRange = totalValue;
            parms2.countRange = countRange;
            parms2.filter = filter;
            parms2.techLevel = techLevel;
            parms2.maxTotalMass = totalMass;

            maker.fixedParams = parms2;

            Rewards = maker.Generate();
        }

        public virtual ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Root, true);

            return filter;
        }

        public virtual void Tick()
        {
            if (!UnlimitedTime)
            {
                TicksToPass--;
                if (TicksToPass <= 0)
                {
                    EndQuest(null, EndCondition.Timeout);
                }
            }
        }

        public virtual void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if(condition == EndCondition.Success)
                GiveRewards(caravan);

            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public virtual void GiveRewards(Caravan caravan)
        {
            if (Rewards == null || Rewards.Count == 0)
                return;

            Map map = Find.AnyPlayerHomeMap;
            IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.DropThingsNear(intVec, map, Rewards, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);
        }

        public virtual void DrawAdditionalOptions(Rect rect)
        {

        }

        public virtual IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            yield break;
        }

        public virtual IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
        {
            yield break;
        }

        public virtual string GetInspectString()
        {
            return null; ;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref TicksToPass, "TicksToPass");
            Scribe_Values.Look(ref UnlimitedTime, "UnlimitedTime");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Deep.Look(ref Target, "Target");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
            Scribe_Collections.Look(ref Rewards, "Rewards", LookMode.Deep);
        }

        public virtual string GetDescription()
        {
            return null;
        }

        public string GetUniqueLoadID()
        {
            return "Quest_" + id;
        }
    }
}
