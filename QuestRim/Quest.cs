﻿using DiaRim;
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
        public abstract QuestDef RelatedQuestDef { get; }

        public QuestSite Site;

        public int id;

        public virtual int SuccessTrustAffect => 0;

        public virtual int FailTrustAffect => 0;

        public virtual int TimeoutTrustAffect => 0;

        public virtual int SuccessAggressiveLevelAffect => 0;

        public virtual int FailAggressiveLevelAffect => 0;

        public virtual int TimeoutAggressiveLevelAffect => 0;

        public virtual bool AffectOnWorld => true;

        public virtual string AdditionalQuestContentString => "AdditionalContent".Translate();

        public abstract string CardLabel { get; }
        public abstract string Description { get; }

        public virtual string ExpandingIconPath { get; }

        public virtual bool UseLeaveCommand => true;

        public bool ShowInConsole = true;

        public DialogDef Dialog;

        public IncidentDef IncidentDef;

        public virtual bool HasExitCells => false;

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

        public List<Thing> Rewards;
        public List<QuestOption> Options;

        public int TicksToPass = 60000;
        public bool UnlimitedTime = false;

        public virtual void SiteTick()
        {

        }

        protected void ResetIcon()
        {
            expandingIcon = null;
        }

        public virtual void PostMapRemove(Map map)
        {

        }

        public virtual void GameLoaded()
        {

        }

        public virtual void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            ShowInConsole = true;

            if (notify)
            {
                Find.LetterStack.ReceiveLetter(QuestsManager.Communications.MakeQuestLetter(this, "TakeQuestFromPawnLetter".Translate(CardLabel), "TakeQuestFromPawnLetterDesc".Translate(CardLabel, Description)));
            }

            quester.Quests.Remove(this);
        }

        public virtual bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            return true;
        }

        public virtual bool CanLeaveFromSite(QuestSite site)
        {
            return true;
        }

        public virtual void PostSiteRemove(QuestSite site)
        {

        }

        public virtual void Notify_CaravanFormed(QuestSite site, Caravan caravan)
        {

        }

        public virtual bool PreForceReform(QuestSite mapParent)
        {
            return true;
        }

        public virtual void PostForceReform(QuestSite mapParent)
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

        public virtual void GenerateRewards()
        {
            Rewards = ThingSetMakerDefOf.Reward_ItemsStandard.root.Generate();
        }

        public void GenerateRewards(ThingSetMakerDef maker, FloatRange totalValue, IntRange? countRange)
        {
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.totalMarketValueRange = totalValue;
            parms.countRange = countRange;

            Rewards = maker.root.Generate(parms);
        }

        public virtual void PostMapGenerate(Map map)
        {
            UnlimitedTime = true;
        }

        public virtual IEnumerable<Gizmo> GetGizmos(QuestSite site)
        {
            yield break;
        }

        public virtual IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            yield break;
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
                    if (Site != null)
                        Site.EndQuest(null, EndCondition.Timeout);
                    else
                        EndQuest(null, EndCondition.Timeout);
                }
            }
        }

        public virtual void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if (condition == EndCondition.Success)
            {
                GiveRewards(caravan);
            }

            if (AffectOnWorld && Faction != null && Faction != Faction.OfPlayer)
            {
                var globalFactionManager = QuestsManager.Communications.FactionManager;
                var interaction = QuestsManager.Communications.FactionManager.GetInteraction(Faction);

                switch (condition)
                {
                    case EndCondition.Success:
                        {
                            TryAffectOnWorld(globalFactionManager, interaction, SuccessAggressiveLevelAffect, SuccessTrustAffect);
                            break;
                        }
                    case EndCondition.Fail:
                        {
                            TryAffectOnWorld(globalFactionManager, interaction, FailAggressiveLevelAffect, FailTrustAffect);
                            break;
                        }
                    case EndCondition.Timeout:
                        {
                            TryAffectOnWorld(globalFactionManager, interaction, TimeoutAggressiveLevelAffect, TimeoutTrustAffect);
                            break;
                        }
                }
            }

            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        protected virtual void TryAffectOnWorld(FactionManager globalFactionManager, FactionInteraction interaction, int aggressiveLevel, int trustLevel)
        {
            if (globalFactionManager != null)
            {
                globalFactionManager.PlayerAggressiveLevel += aggressiveLevel;
            }

            if (interaction != null)
            {
                interaction.Trust += trustLevel;
            }
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
            return null;
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
            Scribe_References.Look(ref Site, "Site");
            Scribe_Defs.Look(ref Dialog, "Dialog");
            Scribe_Values.Look(ref ShowInConsole, "ShowInConsole");
            Scribe_Defs.Look(ref IncidentDef, "IncidentDef");
        }

        public virtual string GetDescription()
        {
            return null;
        }

        public string GetUniqueLoadID()
        {
            return "Quest_" + id;
        }

        public string GetDescriptionWithRewards()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Description);
            float totalPrice = 0;
            Rewards.ForEach(x => totalPrice += x.MarketValue * x.stackCount);

            builder.AppendLine("QuestRewards".Translate(totalPrice));
            if (Rewards != null)
            {
                foreach (var reward in Rewards)
                {
                    builder.AppendLine($"- {reward.Label}");
                }
            }

            return builder.ToString();
        }

        public string GetRewardsString()
        {
            StringBuilder builder = new StringBuilder();
            float totalPrice = 0;
            Rewards.ForEach(x => totalPrice += x.MarketValue * x.stackCount);

            builder.AppendLine("QuestRewards".Translate(totalPrice));
            if (Rewards != null)
            {
                foreach (var reward in Rewards)
                {
                    builder.AppendLine($"- {reward.Label}");
                }
            }

            return builder.ToString();
        }
    }
}
