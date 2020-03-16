using QuestRim.Wars;
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
    public enum Winner : byte
    {
        Assaulters,
        Defenders,
        Draw,
        None
    }

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
        public WarGoalStatWorker StatWorker;

        public int StartTicks;

        public int LastTruceTicks;

        public bool Started => started;
        private bool started;

        private int nextTicksSituations;

        public bool CanGenerateSituationsRightNow => nextTicksSituations < Find.TickManager.TicksGame;

        public List<FactionArmy> Armys => armys;
        protected List<FactionArmy> armys;

        public FactionInteraction PlayerInteraction => QuestsManager.Communications.FactionManager.PlayerInteraction;

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
            if (attackedAlliance != null)
            {
                foreach (var faction in attackedAlliance.Factions)
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

        public void Tick()
        {
            if (Started && CanGenerateSituationsRightNow)
            {
                GenerateSituations();
            }

            Worker.Tick();
        }

        public List<FactionInteraction> GetEnemyList(FactionInteraction factionInteraction)
        {
            if (factionInteraction == DeclaredWarFaction || AssaultFactions.Contains(factionInteraction))
                return DefendingFactions;

            if (factionInteraction == DefendingFaction || DefendingFactions.Contains(factionInteraction))
                return AssaultFactions;

            return null;
        }


        public void GenerateSituations()
        {
            nextTicksSituations = Find.TickManager.TicksGame + (Rand.Range(2, 5) * 60000);

            bool playerWar = DeclaredWarFaction == PlayerInteraction || DefendingFaction == PlayerInteraction;
            List<FactionArmy> avaliableArmys = armys.Where(x => (playerWar && x.UseForGenerator) || !playerWar).ToList();
            if (avaliableArmys == null || avaliableArmys.Count == 0)
                return;

            int incidentCounts = Rand.Range(1, Mathf.Max(1, (int)(avaliableArmys.Count * 0.8f)));
            for (int i = 0; i < incidentCounts; i++)
            {
                var army = avaliableArmys.RandomElement();

                if (TryFindArmyOrderFor(army, out ArmyOrderDef armyOrder))
                {
                    if (armyOrder.Worker.GiveTo(army, this))
                    {
                        int ticksGame = Find.TickManager.TicksGame;
                        if (army.lastGiveOrderTicks.ContainsKey(armyOrder))
                        {
                            army.lastGiveOrderTicks[armyOrder] = ticksGame;
                        }
                        else
                        {
                            army.lastGiveOrderTicks.Add(armyOrder, ticksGame);
                        }
                    }
                }
            }

            return;
        }

        private bool TryFindArmyOrderFor(FactionArmy army, out ArmyOrderDef armyOrder)
        {
            armyOrder = null;
            foreach (var order in DefDatabase<ArmyOrderDef>.AllDefs)
            {
                if (order.targetGoals.Contains(WarGoalDef))
                {
                    if (order.Worker.CanGive(army, this))
                    {
                        armyOrder = order;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CanTruceRightNow()
        {
            if(Worker.CanTruceRightNow())
            {
                int delay = LastTruceTicks - Find.TickManager.TicksGame;
                if (delay <= 0)
                    return true;
            }

            return false;
        }

        public void AddAssaultFaction(FactionInteraction factionInteraction)
        {
            if (AssaultFactions.Contains(factionInteraction))
                return;

            AssaultFactions.Add(factionInteraction);

            Notify_NewFactionEntered(factionInteraction);
        }

        public void AddDefendFaction(FactionInteraction factionInteraction)
        {
            if (DefendingFactions.Contains(factionInteraction))
                return;

            DefendingFactions.Add(factionInteraction);

            Notify_NewFactionEntered(factionInteraction);
        }

        public void Notify_NewFactionEntered(FactionInteraction factionInteraction)
        {
            Worker.NewFactionEntered(factionInteraction);

            bool useForGenerator = true;
            if ((DeclaredWarFaction == PlayerInteraction && AssaultFactions.Contains(factionInteraction))
                || DefendingFaction == PlayerInteraction && DefendingFactions.Contains(factionInteraction))
            {
                useForGenerator = false;
            }

            AddArmy(CreateArmyFor(factionInteraction, useForGenerator));

            factionInteraction.Notify_WarIsStarted(this);
        }

        public void Notify_FactionLeft(FactionInteraction factionInteraction)
        {
            Worker.FactionLeft(factionInteraction);

            RemoveArmy(factionInteraction);

            factionInteraction.Notify_WarIsOver(this);
        }

        public void RemoveAssaultFaction(FactionInteraction factionInteraction)
        {
            if (!AssaultFactions.Contains(factionInteraction))
                return;

            AssaultFactions.Remove(factionInteraction);

            Notify_FactionLeft(factionInteraction);
        }

        public void RemoveDefendFaction(FactionInteraction factionInteraction)
        {
            if (!DefendingFactions.Contains(factionInteraction))
                return;

            DefendingFactions.Remove(factionInteraction);

            Notify_FactionLeft(factionInteraction);
        }

        public void AddArmy(FactionArmy factionArmy)
        {
            armys.Add(factionArmy);
        }

        private void InitializeArmy()
        {
            armys = new List<FactionArmy>();

            bool useAttackersForGenerator = DeclaredWarFaction != PlayerInteraction;
            bool useDefendersForGenerator = DefendingFaction != PlayerInteraction;

            foreach (var assaulter in AssaultFactions)
            {
                AddArmy(CreateArmyFor(assaulter, useAttackersForGenerator));
            }

            foreach (var defender in DefendingFactions)
            {
                AddArmy(CreateArmyFor(defender, useDefendersForGenerator));
            }
        }

        public void RemoveArmy(FactionInteraction factionInteraction)
        {
            for (int i = 0; i < armys.Count; i++)
            {
                FactionArmy army = armys[i];

                if (army.Faction == factionInteraction)
                {
                    armys.Remove(army);

                    break;
                }
            }
        }

        public FactionArmy CreateArmyFor(FactionInteraction factionInteraction, bool useForGenerator)
        {
            var army = new FactionArmy(factionInteraction, this, useForGenerator);
            army.DepletionFromWar = 0;

            return army;
        }

        public void EndWar(Winner winner)
        {
            foreach (var faction1 in AssaultFactions)
                faction1.Notify_WarIsOver(this);

            foreach (var faction2 in DefendingFactions)
                faction2.Notify_WarIsOver(this);

            Worker.EndWar(winner);

            QuestsManager.Communications.FactionManager.Remove(this);

            started = false;

            Find.LetterStack.ReceiveLetter("War_EndWarTitle".Translate(WarName), "War_EndWarDesc".Translate(WarName, DeclaredWarFaction.Faction.Name, DefendingFaction.Faction.Name), LetterDefOf.PositiveEvent);
        }

        public void StartWar(bool notify = true)
        {
            if (Started)
                return;

            if (WarGoalDef == null)
                return;

            if (AssaultFactions == null || DefendingFactions == null)
                return;

            LongEventHandler.QueueLongEvent(delegate
            {
                StartWarNow(notify);
            }, "War_LongEventStarting", doAsynchronously: true, null);
        }

        private void StartWarNow(bool notify = true)
        {
            InitializeArmy();

            Worker = (WarGoalWorker)Activator.CreateInstance(WarGoalDef.workerClass);
            StatWorker = (WarGoalStatWorker)Activator.CreateInstance(WarGoalDef.statClass);

            StatWorker.Initialize(this);
            Worker.StartWar(this);

            foreach (var faction1 in AssaultFactions)
            {
                foreach (var defender in DefendingFactions)
                {
                    faction1.Faction.TryAffectGoodwillWith(defender.Faction, -1000);

                    if(DefendingFaction == PlayerInteraction)
                        faction1.Trust = -50;
                    else if(DeclaredWarFaction == PlayerInteraction)
                        defender.Trust = -50;
                }

                faction1.Notify_WarIsStarted(this);
            }

            foreach (var faction2 in DefendingFactions)
                faction2.Notify_WarIsStarted(this);

            var attackedAlliance = AttackedAlliance;
            if (attackedAlliance != null)
            {
                attackedAlliance.AffectGoodwillWith(-15);
            }

            Find.LetterStack.ReceiveLetter("War_WarIsStartedTitle".Translate(WarName), "War_WarIsStartedDesc".Translate(DeclaredWarFaction.Faction.Name, AttackedAlliance == null ? "WarManager_NoAlliance".Translate().ToString() : AttackedAlliance.Name, DefendingFaction.Faction.Name, DefendAlliance == null ? "WarManager_NoAlliance".Translate().ToString() : DefendAlliance.Name, WarName, WarGoalDef.LabelCap), LetterDefOf.ThreatBig);

            nextTicksSituations = Find.TickManager.TicksGame + 25000;

            started = true;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");

            Scribe_References.Look(ref DeclaredWarFaction, "DeclaredWarFaction");
            Scribe_References.Look(ref DefendingFaction, "DefendingFaction");
            Scribe_Defs.Look(ref WarGoalDef, "WarGoalDef");

            Scribe_Deep.Look(ref Worker, "Worker");
            Scribe_Deep.Look(ref StatWorker, "StatWorker");

            Scribe_Collections.Look(ref AssaultFactions, "AssaultFactions", LookMode.Reference);
            Scribe_Collections.Look(ref DefendingFactions, "DefendingFactions", LookMode.Reference);
            Scribe_Values.Look(ref StartTicks, "StartTicks");
            Scribe_Values.Look(ref started, "started");
            Scribe_Values.Look(ref WarName, "WarName");

            Scribe_Values.Look(ref LastTruceTicks, "LastTruceTicks");
            Scribe_Values.Look(ref nextTicksSituations, "nextTicksSituations");
            Scribe_Collections.Look(ref armys, "armys", LookMode.Deep);

        }

        public string GetUniqueLoadID()
        {
            return "War_" + id;
        }
    }
}
