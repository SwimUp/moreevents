using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class War : IExposable, ILoadReferenceable
    {
        public int id;

        public string WarName;

        public FactionInteraction DeclaredWarFaction;

        public FactionInteraction DefendingFaction;

        public List<FactionInteraction> AssaultFactions;

        public List<FactionInteraction> DefendingFactions;

        public Alliance AttackedAlliance => DeclaredWarFaction.Alliance;

        public Alliance DefendAlliance => DefendingFaction.Alliance;

        public WarGoalDef WarGoalDef;
        public WarGoalWorker Worker;

        public int StartTicks;

        public bool Started => started;
        private bool started;

        public War()
        {

        }

        public War(string warName, WarGoalDef goal, FactionInteraction declaredWarFaction, FactionInteraction defendingFaction)
        {
            DeclaredWarFaction = declaredWarFaction;
            DefendingFaction = defendingFaction;

            WarGoalDef = goal;
            WarName = warName;

            AssaultFactions = new List<FactionInteraction>() { declaredWarFaction };
            var attackedAlliance = AttackedAlliance;
            if(attackedAlliance != null)
            {
                foreach(var faction in attackedAlliance.Factions)
                {
                    if (faction == DeclaredWarFaction)
                        continue;

                    AssaultFactions.Add(faction);
                }
            }

            DefendingFactions = new List<FactionInteraction> { defendingFaction };
            var defendAlliance = DefendAlliance;
            if (defendAlliance != null)
            {
                foreach (var faction in defendAlliance.Factions)
                {
                    if (faction == DefendingFaction)
                        continue;

                    DefendingFactions.Add(faction);
                }
            }

            StartTicks = Find.TickManager.TicksGame;
        }

        public void AddAssaultFaction(FactionInteraction factionInteraction)
        {
            if (AssaultFactions.Contains(factionInteraction))
                return;

            AssaultFactions.Add(factionInteraction);

            factionInteraction.Notify_WarIsStarted(this);
        }

        public void AddDefendFaction(FactionInteraction factionInteraction)
        {
            if (DefendingFactions.Contains(factionInteraction))
                return;

            DefendingFactions.Add(factionInteraction);

            factionInteraction.Notify_WarIsStarted(this);
        }

        public void RemoveAssaultFaction(FactionInteraction factionInteraction)
        {
            if (!AssaultFactions.Contains(factionInteraction))
                return;

            AssaultFactions.Remove(factionInteraction);

            factionInteraction.Notify_WarIsOver(this);
        }

        public void RemoveDefendFaction(FactionInteraction factionInteraction)
        {
            if (!DefendingFactions.Contains(factionInteraction))
                return;

            DefendingFactions.Remove(factionInteraction);

            factionInteraction.Notify_WarIsOver(this);
        }

        public void EndWar()
        {
            foreach (var faction1 in AssaultFactions)
                faction1.Notify_WarIsOver(this);

            foreach (var faction2 in DefendingFactions)
                faction2.Notify_WarIsOver(this);
        }

        public void StartWar(bool notify = true)
        {
            if (Started)
                return;

            if (WarGoalDef == null)
                return;

            if (AssaultFactions == null || DefendingFactions == null)
                return;

            Worker = (WarGoalWorker)Activator.CreateInstance(WarGoalDef.workerClass);

            Worker.StartWar(this);

            foreach (var faction1 in AssaultFactions)
                faction1.Notify_WarIsStarted(this);

            foreach (var faction2 in DefendingFactions)
                faction2.Notify_WarIsStarted(this);

            Find.LetterStack.ReceiveLetter("War_WarIsStartedTitle".Translate(WarName), "War_WarIsStartedDesc".Translate(DeclaredWarFaction.Faction.Name, AttackedAlliance == null ? "WarManager_NoAlliance".Translate() : AttackedAlliance.Name, DefendingFaction.Faction.Name, DefendAlliance == null ? "WarManager_NoAlliance".Translate() : DefendAlliance.Name, WarName, WarGoalDef.LabelCap), LetterDefOf.NeutralEvent);

            started = true;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");

            Scribe_References.Look(ref DeclaredWarFaction, "DeclaredWarFaction");
            Scribe_References.Look(ref DefendingFaction, "DefendingFaction");
            Scribe_Defs.Look(ref WarGoalDef, "WarGoalDef");

            Scribe_Deep.Look(ref Worker, "Worker");

            Scribe_Collections.Look(ref AssaultFactions, "AssaultFactions", LookMode.Reference);
            Scribe_Collections.Look(ref DefendingFactions, "DefendingFactions", LookMode.Reference);
            Scribe_Values.Look(ref StartTicks, "StartTicks");
            Scribe_Values.Look(ref started, "started");
            Scribe_Values.Look(ref WarName, "WarName");
        }

        public string GetUniqueLoadID()
        {
            return "War_" + id;
        }
    }
}
