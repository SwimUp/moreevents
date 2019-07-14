using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class ScoutingComp : CommunicationComponent
    {
        private static Dictionary<IIncidentTarget, StoryState> tmpOldStoryStates = new Dictionary<IIncidentTarget, StoryState>();

        private List<IncidentCategoryDef> allowedCategories = new List<IncidentCategoryDef>
        {
            IncidentCategoryDefOf.ThreatBig,
            IncidentCategoryDefOf.ThreatSmall,
            IncidentCategoryDefOf.RaidBeacon
        };

        public Faction Faction;
        private int ticksBetweenInfo;
        private int maxScoutTicks;
        private int scoutDays;

        public ScoutingComp()
        {

        }

        public void AddDays(int day)
        {
            maxScoutTicks += day * 60000;
        }

        public string GetDays()
        {
            return maxScoutTicks.TicksToDays().ToString("f2");
        }

        public static bool ScoutAlready(Faction scoutFaction, out ScoutingComp outComp)
        {
            outComp = null;
            foreach (var comp in QuestsManager.Communications.Components)
            {
                if(comp is ScoutingComp scoutComp && scoutComp.Faction == scoutFaction)
                {
                    outComp = scoutComp;
                    return true;
                }
            }

            return false;
        }

        public static void GiveScoutingComp(Faction faction, int delayDays, int totalDays, int scoutDays, int addDaysIfAlreadyHas = 5)
        {
            if (!ScoutingComp.ScoutAlready(faction, out ScoutingComp outComp))
            {
                ScoutingComp comp = new ScoutingComp(faction, delayDays * 60000, totalDays * 60000, scoutDays);
                comp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();

                QuestsManager.Communications.RegisterComponent(comp);

                FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(faction);
                foreach(var opt in interaction.Options)
                {
                    if(opt is CommOption_SubscribeScout opt2)
                    {
                        opt2.Active = true;
                    }
                }

                Find.LetterStack.ReceiveLetter("ScoutComp_GiveTitle".Translate(), "ScoutComp_Give".Translate(faction.Name, delayDays, totalDays), LetterDefOf.PositiveEvent);
            }
            else
            {
                outComp.AddDays(addDaysIfAlreadyHas);
                Find.LetterStack.ReceiveLetter("ScoutComp_AlreadyGiveTitle".Translate(), "ScoutComp_AlreadyGive".Translate(faction.Name, addDaysIfAlreadyHas), LetterDefOf.PositiveEvent);
            }
        }

        public override void Tick()
        {
            maxScoutTicks--;

            if(maxScoutTicks <= 0)
            {
                EndComp();
                return;
            }

            if(Find.TickManager.TicksGame % ticksBetweenInfo == 0)
            {
                GenerateScoutInfo();
            }
        }

        public void EndComp()
        {
            FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(Faction);
            foreach (var opt in interaction.Options)
            {
                if (opt is CommOption_SubscribeScout opt2)
                {
                    opt2.Active = false;
                }
            }

            QuestsManager.Communications.RemoveComponent(this);
        }

        public void GenerateScoutInfo()
        {
            StringBuilder builder = new StringBuilder();
            List<StorytellerComp> storytellerComps = Find.Storyteller.storytellerComps;
            for (int i = 0; i < storytellerComps.Count; i++)
            {
                StorytellerComp comp = storytellerComps[i];
                if(comp is StorytellerComp_OnOffCycle onOffCycle)
                {
                    StorytellerCompProperties_OnOffCycle prop = (StorytellerCompProperties_OnOffCycle)onOffCycle.props;
                    GetFutureIncidentsLight(scoutDays, Find.AnyPlayerHomeMap, out List<Pair<IncidentDef, IncidentParms>> allIncidents, out List<float> daysToEvents);
                    if (allIncidents.Count > 0)
                    {
                        builder.Append("ScoutingComp_InfoTitle".Translate());
                        for (int i2 = 0; i2 < allIncidents.Count; i2++)
                        {
                            Pair<IncidentDef, IncidentParms> pair = allIncidents[i2];

                            if (!allowedCategories.Contains(pair.First.category))
                                continue;

                            builder.Append($"- {pair.First.LabelCap} ");
                            if (pair.Second.points > 0f && Rand.Chance(0.10f * (int)Faction.def.techLevel))
                            {
                                builder.Append("ScoutingComp_ThreatKnown".Translate(pair.Second.points.ToString("f2")));
                            }
                            else
                            {
                                builder.Append("ScoutingComp_ThreatUnknown".Translate());
                            }

                            float time = Rand.Range(daysToEvents[i2] * 0.7f, daysToEvents[i2] * 1.4f);
                            builder.Append("ScoutingComp_CommingTime".Translate(time.ToString("f2")));
                            builder.Append("\n");
                        }
                    }
                    else
                    {
                        builder.Append("ScoutingComp_NoThreatsInfoTitle".Translate());
                    }

                    EmailMessage emailMessage = QuestsManager.Communications.PlayerBox.FormMessageFrom(Faction, builder.ToString(), "ScoutingComp_Subject".Translate());
                    QuestsManager.Communications.PlayerBox.SendMessage(emailMessage);

                    return;
                }
            }
        }
    
        public ScoutingComp(Faction scoutFaction, int ticksBetweenInfo, int maxScoutTicks, int scoutDays)
        {
            Faction = scoutFaction;
            this.ticksBetweenInfo = ticksBetweenInfo;
            this.maxScoutTicks = maxScoutTicks;
            this.scoutDays = scoutDays;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Values.Look(ref ticksBetweenInfo, "ticksBetweenInfo");
            Scribe_Values.Look(ref maxScoutTicks, "maxScoutTicks");
            Scribe_Values.Look(ref scoutDays, "scoutDays");
        }

        public static void GetFutureIncidents(int numDays, Map forMap, out Dictionary<IIncidentTarget, int> incCountsForTarget, out int[] incCountsForComp, out List<Pair<IncidentDef, IncidentParms>> allIncidents, out int threatBigCount, out List<float> daysToEvents, StringBuilder outputSb = null, StorytellerComp onlyThisComp = null)
        {
            int ticksGame = Find.TickManager.TicksGame;
            daysToEvents = new List<float>();
            IncidentQueue incidentQueue = Find.Storyteller.incidentQueue;
            List<IIncidentTarget> allIncidentTargets = Find.Storyteller.AllIncidentTargets;
            tmpOldStoryStates.Clear();
            for (int i = 0; i < allIncidentTargets.Count; i++)
            {
                IIncidentTarget incidentTarget = allIncidentTargets[i];
                tmpOldStoryStates.Add(incidentTarget, incidentTarget.StoryState);
                new StoryState(incidentTarget).CopyTo(incidentTarget.StoryState);
            }
            Find.Storyteller.incidentQueue = new IncidentQueue();
            int num = numDays * 60;
            incCountsForComp = new int[Find.Storyteller.storytellerComps.Count];
            incCountsForTarget = new Dictionary<IIncidentTarget, int>();
            allIncidents = new List<Pair<IncidentDef, IncidentParms>>();
            threatBigCount = 0;
            for (int j = 0; j < num; j++)
            {
                IEnumerable<FiringIncident> enumerable = (onlyThisComp == null) ? Find.Storyteller.MakeIncidentsForInterval() : Find.Storyteller.MakeIncidentsForInterval(onlyThisComp, Find.Storyteller.AllIncidentTargets);
                foreach (FiringIncident item in enumerable)
                {
                    if (item == null)
                    {
                        Log.Error("Null incident generated.");
                    }
                    if (item.parms.target == forMap)
                    {
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        allIncidents.Add(new Pair<IncidentDef, IncidentParms>(item.def, item.parms));
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        if (!incCountsForTarget.ContainsKey(item.parms.target))
                        {
                            incCountsForTarget[item.parms.target] = 0;
                        }
                        Dictionary<IIncidentTarget, int> dictionary;
                        IIncidentTarget target;
                        (dictionary = incCountsForTarget)[target = item.parms.target] = dictionary[target] + 1;
                        if (item.def.category == IncidentCategoryDefOf.ThreatBig || item.def.category == IncidentCategoryDefOf.RaidBeacon)
                        {
                            threatBigCount++;
                        }
                        int num2 = Find.Storyteller.storytellerComps.IndexOf(item.source);
                        incCountsForComp[num2]++;
                        daysToEvents.Add(Find.TickManager.TicksGame.TicksToDays());
                    }
                }
                Find.TickManager.DebugSetTicksGame(Find.TickManager.TicksGame + 1000);
            }
            Find.TickManager.DebugSetTicksGame(ticksGame);
            Find.Storyteller.incidentQueue = incidentQueue;
            for (int k = 0; k < allIncidentTargets.Count; k++)
            {
                tmpOldStoryStates[allIncidentTargets[k]].CopyTo(allIncidentTargets[k].StoryState);
            }
            tmpOldStoryStates.Clear();
        }

        public static void GetFutureIncidentsLight(int numDays, Map forMap, out List<Pair<IncidentDef, IncidentParms>> allIncidents, out List<float> daysToEvents, StorytellerComp onlyThisComp = null)
        {
            int ticksGame = Find.TickManager.TicksGame;
            daysToEvents = new List<float>();
            IncidentQueue incidentQueue = Find.Storyteller.incidentQueue;
            List<IIncidentTarget> allIncidentTargets = Find.Storyteller.AllIncidentTargets;
            tmpOldStoryStates.Clear();
            for (int i = 0; i < allIncidentTargets.Count; i++)
            {
                IIncidentTarget incidentTarget = allIncidentTargets[i];
                tmpOldStoryStates.Add(incidentTarget, incidentTarget.StoryState);
                new StoryState(incidentTarget).CopyTo(incidentTarget.StoryState);
            }
            Find.Storyteller.incidentQueue = new IncidentQueue();
            int num = numDays * 60;
            allIncidents = new List<Pair<IncidentDef, IncidentParms>>();
            for (int j = 0; j < num; j++)
            {
                IEnumerable<FiringIncident> enumerable = (onlyThisComp == null) ? Find.Storyteller.MakeIncidentsForInterval() : Find.Storyteller.MakeIncidentsForInterval(onlyThisComp, Find.Storyteller.AllIncidentTargets);
                foreach (FiringIncident item in enumerable)
                {
                    if (item == null)
                    {
                        Log.Error("Null incident generated.");
                    }
                    if (item.parms.target == forMap)
                    {
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        allIncidents.Add(new Pair<IncidentDef, IncidentParms>(item.def, item.parms));
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        int num2 = Find.Storyteller.storytellerComps.IndexOf(item.source);
                        daysToEvents.Add(Find.TickManager.TicksGame.TicksToDays());
                    }
                }
                Find.TickManager.DebugSetTicksGame(Find.TickManager.TicksGame + 1000);
            }
            Find.TickManager.DebugSetTicksGame(ticksGame);
            Find.Storyteller.incidentQueue = incidentQueue;
            for (int k = 0; k < allIncidentTargets.Count; k++)
            {
                tmpOldStoryStates[allIncidentTargets[k]].CopyTo(allIncidentTargets[k].StoryState);
            }
            tmpOldStoryStates.Clear();
        }
    }
}
