using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    public class Quest_BanditCamp : Quest
    {
        public override string CardLabel => "Quest_BanditCamp_CardLabel".Translate();

        public override string Description => questText;
        private string questText;

        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_BanditCamp;

        public Site QuestSite;

        public Quest_BanditCamp() { }

        public Quest_BanditCamp(string text, Site site)
        {
            questText = text;
            QuestSite = site;
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if(condition == EndCondition.None && QuestSite != null)
            {
                Find.WorldObjects.Remove(QuestSite);
            }
            
            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public override void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            QuestSite.GetComponent<TimeoutComp>().StartTimeout(TicksToPass);

            base.TakeQuestByQuester(quester, notify);
        }

        public override bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            if (!TryFindFactions(out Faction alliedFaction, out Faction enemyFaction))
            {
                return false;
            }
            if (!TryFindTile(out int tile))
            {
                return false;
            }

            Site site = SiteMaker.MakeSite(SiteCoreDefOf.Nothing, SitePartDefOf.Outpost, tile, enemyFaction);
            site.sitePartsKnown = true;
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.totalMarketValueRange = SiteTuning.BanditCampQuestRewardMarketValueRange * SiteTuning.QuestRewardMarketValueThreatPointsFactor.Evaluate(site.desiredThreatPoints);
            List<Thing> list = ThingSetMakerDefOf.Reward_StandardByDropPod.root.Generate(parms);
            site.GetComponent<DefeatAllEnemiesQuestComp>().StartQuest(alliedFaction, 18, list);
            int randomInRange = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;

            IncidentDef def = IncidentDefOfLocal.Quest_BanditCamp;
            string text = def.letterText.Formatted(alliedFaction.leader.LabelShort, alliedFaction.def.leaderTitle, alliedFaction.Name, GenLabel.ThingsLabel(list, string.Empty), randomInRange.ToString(), SitePartUtility.GetDescriptionDialogue(site, site.parts.FirstOrDefault()), GenThing.GetMarketValue(list).ToStringMoney()).CapitalizeFirst();
            GenThing.TryAppendSingleRewardInfo(ref text, list);
            Find.LetterStack.ReceiveLetter(def.letterLabel, text, def.letterDef, site, alliedFaction);

            questText = text;
            TicksToPass = randomInRange * 60000;
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            Faction = alliedFaction;
            Target = site;

            QuestSite = site;
            Rewards = new List<Thing>();

            ShowInConsole = false;

            Find.WorldObjects.Add(QuestSite);
            QuestsManager.Communications.AddQuestPawn(questPawn, this);
            QuestsManager.Communications.AddQuest(this);

            return true;
        }

        private bool TryFindTile(out int tile)
        {
            IntRange banditCampQuestSiteDistanceRange = SiteTuning.BanditCampQuestSiteDistanceRange;
            return TileFinder.TryFindNewSiteTile(out tile, banditCampQuestSiteDistanceRange.min, banditCampQuestSiteDistanceRange.max);
        }

        public bool TryFindFactions(out Faction alliedFaction, out Faction enemyFaction)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where !x.def.hidden && !x.defeated && !x.IsPlayer && !x.HostileTo(Faction.OfPlayer) && CommonHumanlikeEnemyFactionExists(Faction.OfPlayer, x) && !IncidentWorker_QuestBanditCamp.AnyQuestExistsFrom(x)
                 select x).TryRandomElement(out alliedFaction))
            {
                enemyFaction = CommonHumanlikeEnemyFaction(Faction.OfPlayer, alliedFaction);
                return true;
            }
            alliedFaction = null;
            enemyFaction = null;
            return false;
        }

        private bool CommonHumanlikeEnemyFactionExists(Faction f1, Faction f2)
        {
            return CommonHumanlikeEnemyFaction(f1, f2) != null;
        }

        private Faction CommonHumanlikeEnemyFaction(Faction f1, Faction f2)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where x != f1 && x != f2 && !x.def.hidden && x.def.humanlikeFaction && !x.defeated && x.HostileTo(f1) && x.HostileTo(f2)
                 select x).TryRandomElement(out Faction result))
            {
                return result;
            }
            return null;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref questText, "text");
            Scribe_References.Look(ref QuestSite, "QuestSite");
        }
    }
}
