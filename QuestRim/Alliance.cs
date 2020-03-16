using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public enum AllianceRemoveReason : byte
    {
        Kick,
        LowTrust,
        None
    }

    public class Alliance : IExposable, ILoadReferenceable, IIncidentTarget
    {
        public int id;

        public bool PlayerOwner;

        public string Name;

        public Faction FactionOwner;

        public AllianceGoalDef AllianceGoalDef;

        public List<FactionInteraction> Factions = new List<FactionInteraction>();

        public IEnumerable<Quest> AllianceQuests => QuestsManager.Communications.Quests.Where(x => Factions.Contains(x.Faction) && x.ShowInConsole);

        private List<StorytellerComp> storytellerComps;

        public int Tile => Find.Maps.First(x => x.ParentFaction == FactionOwner).Tile;

        public StoryState StoryState => storyState;
        private StoryState storyState;

        public GameConditionManager GameConditionManager
        {
            get
            {
                Log.ErrorOnce("Attempted to retrieve condition manager directly from alliance", 16427530);
                return null;
            }
        }

        public float PlayerWealthForStoryteller => 0f;

        public IEnumerable<Pawn> PlayerPawnsForStoryteller => Enumerable.Empty<Pawn>();

        public FloatRange IncidentPointsRandomFactorRange => new FloatRange(1f, 0.9f);

        public int ConstantRandSeed => 0;

        public const int KickTrustChange = -100;

        public List<AllianceAgreementComp> AllianceAgreements
        {
            get
            {
                if (allianceAgreements == null)
                {
                    allianceAgreements = new List<AllianceAgreementComp>();
                }

                return allianceAgreements;
            }
        }
        private List<AllianceAgreementComp> allianceAgreements;

        public int AgreementsSlots => 3;

        public IEnumerable<War> Wars => QuestsManager.Communications.FactionManager.Wars.Where(x => x.AttackedAlliance == this || x.DefendAlliance == this);

        public Alliance()
        {

        }

        public Alliance(string name, Faction faction, AllianceGoalDef goal)
        {
            Name = name;
            FactionOwner = faction;
            AllianceGoalDef = goal;
            storyState = new StoryState(this);

            InitializeStorytellerComps();
        }

        private void InitializeStorytellerComps()
        {
            storytellerComps = new List<StorytellerComp>();
            for (int i = 0; i < AllianceGoalDef.comps.Count; i++)
            {
                StorytellerComp storytellerComp = (StorytellerComp)Activator.CreateInstance(AllianceGoalDef.comps[i].compClass);
                storytellerComp.props = AllianceGoalDef.comps[i];
                storytellerComps.Add(storytellerComp);
            }
        }

        public IEnumerable<FiringIncident> MakeIncidentsForInterval()
        {
            for (int i = 0; i < storytellerComps.Count; i++)
            {
                foreach (FiringIncident item in MakeIncidentsForInterval(storytellerComps[i], this))
                {
                    yield return item;
                }
            }
        }

        public void Tick()
        {
            if(AllianceAgreements != null)
            {
                for(int i = 0; i < AllianceAgreements.Count; i++)
                {
                    AllianceAgreements[i].Tick();
                }
            }
        }

        public bool AddAgreement(AllianceAgreementDef allianceAgreementDef, FactionInteraction signedFaction, int endTicks)
        {
            if (!CanSignAgreement(allianceAgreementDef, out string reason))
            {
                Messages.Message("Alliance_AddAgreement_False".Translate(reason), MessageTypeDefOf.NeutralEvent);
                return false;
            }

            AllianceAgreementComp allianceAgreementComp = (AllianceAgreementComp)Activator.CreateInstance(allianceAgreementDef.Comp.compClass);
            allianceAgreementComp.AllianceAgreementDef = allianceAgreementDef;
            allianceAgreementComp.SignedFaction = signedFaction;
            allianceAgreementComp.OwnerFaction = QuestsManager.Communications.FactionManager.GetInteraction(FactionOwner);
            allianceAgreementComp.EndTicks = Find.TickManager.TicksGame + endTicks;
            allianceAgreementComp.SignTicks = Find.TickManager.TicksGame;
            allianceAgreementComp.Alliance = this;

            AllianceAgreements.Add(allianceAgreementComp);

            return true;
        }

        public bool AddAgreement(AllianceAgreementComp allianceAgreementComp)
        {
            AllianceAgreements.Add(allianceAgreementComp);

            return true;
        }

        public bool CanSignAgreement(AllianceAgreementDef allianceAgreementDef, out string reason)
        {
            reason = string.Empty;

            if (AgreementActive(allianceAgreementDef))
            {
                reason = "Alliance_CanSignAgreement_AlreadyActive".Translate();
                return false;
            }

            if (!allianceAgreementDef.TargetGoals.Contains(AllianceGoalDef))
            {
                reason = "Alliance_CanSignAgreement_NoTargetGoal".Translate();
                return false;
            }

            if (allianceAgreementDef.UseAgreementsSlot && allianceAgreements.Count == AgreementsSlots)
            {
                reason = "Alliance_CanSignAgreement_NoSlots".Translate(AgreementsSlots);
                return false;
            }

            if(allianceAgreementDef.Conditions != null)
            {
                foreach(var condition in allianceAgreementDef.Conditions)
                {
                    if (!condition.Avaliable(this))
                    {
                        reason = condition.Reason;
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AgreementActive(AllianceAgreementDef allianceAgreementDef)
        {
            return AllianceAgreements.Any(x => x.AllianceAgreementDef == allianceAgreementDef);
        }

        public void EndAgreement(AllianceAgreementComp allianceAgreementComp, AgreementEndReason agreementEndReason)
        {
            if(AllianceAgreements.Contains(allianceAgreementComp))
            {
                allianceAgreementComp.End(agreementEndReason);

                AllianceAgreements.Remove(allianceAgreementComp);
            }
        }

        public AllianceAgreementComp GetAgreement(AllianceAgreementDef allianceAgreementDef)
        {
            return AllianceAgreements.FirstOrDefault(x => x.AllianceAgreementDef == allianceAgreementDef);
        }

        public IEnumerable<FiringIncident> MakeIncidentsForInterval(StorytellerComp comp, IIncidentTarget targ)
        {
            if (GenDate.DaysPassedFloat <= comp.props.minDaysPassed)
            {
                yield break;
            }
            bool flag = false;
            bool flag2 = comp.props.allowedTargetTags.NullOrEmpty();
            foreach (IncidentTargetTagDef item in targ.IncidentTargetTags())
            {
                if (!comp.props.disallowedTargetTags.NullOrEmpty() && comp.props.disallowedTargetTags.Contains(item))
                {
                    flag = true;
                    break;
                }
                if (!flag2 && comp.props.allowedTargetTags.Contains(item))
                {
                    flag2 = true;
                }
            }
            if (!flag && flag2)
            {
                foreach (FiringIncident fi in comp.MakeIntervalIncidents(targ))
                {
                    if (Find.Storyteller.difficulty.allowBigThreats || (fi.def.category != IncidentCategoryDefOf.ThreatBig))
                    {
                        yield return fi;
                    }
                }
            }
        }

        public void AddFaction(FactionInteraction faction)
        {
            Factions.Add(faction);

            SynchonizeAllOwnerRelations();

            AddIntoWars(faction);

            Find.LetterStack.ReceiveLetter("Alliance_AddFactionTitle".Translate(faction.Faction.Name), "Alliance_AddFactionDesc".Translate(faction.Faction.Name), LetterDefOf.PositiveEvent);
        }
        
        public void AddIntoWars(FactionInteraction faction)
        {
            FactionInteraction owner = QuestsManager.Communications.FactionManager.GetInteraction(FactionOwner);
            foreach (var war in faction.InWars)
            {
                if (war.DeclaredWarFaction == faction)
                {
                    Factions.ForEach(fac =>
                    {
                        war.AddAssaultFaction(fac);
                    });

                    war.AddAssaultFaction(owner);
                }
                else if (war.DefendingFaction == faction)
                {
                    Factions.ForEach(fac =>
                    {
                        war.AddDefendFaction(fac);
                    });

                    war.AddDefendFaction(owner);
                }
            }

            foreach (var war in owner.InWars)
            {
                if (war.DeclaredWarFaction == owner)
                {
                    war.AddAssaultFaction(faction);
                }
                else if (war.DefendingFaction == owner)
                {
                    war.AddDefendFaction(faction);
                }
            }
        }

        public void RemoveFromWars(FactionInteraction faction)
        {
            FactionInteraction owner = QuestsManager.Communications.FactionManager.GetInteraction(FactionOwner);
            foreach (var war in faction.InWars)
            {
                if (war.DeclaredWarFaction == faction)
                {
                    Factions.ForEach(fac =>
                    {
                        war.RemoveAssaultFaction(fac);
                    });

                    war.RemoveAssaultFaction(owner);
                }
                else if(war.DefendingFaction == faction)
                {
                    Factions.ForEach(fac =>
                    {
                        war.RemoveDefendFaction(fac);
                    });

                    war.RemoveDefendFaction(owner);
                }
            }

            foreach (var war in owner.InWars)
            {
                if (war.DeclaredWarFaction == owner)
                {
                    war.RemoveAssaultFaction(faction);
                }
                else if (war.DefendingFaction == owner)
                {
                    war.RemoveDefendFaction(faction);
                }
            }
        }

        public void GiveTrustToAllFactions(int trust)
        {
            List<FactionInteraction> factions = new List<FactionInteraction>(Factions);

            foreach(var faction in factions)
            {
                faction.Trust += trust;
            }
        }

        public void AffectGoodwillWith(int goodwill)
        {
            List<FactionInteraction> interactions = new List<FactionInteraction>(Factions);

            foreach(var faction in interactions)
            {
                faction.Faction.TryAffectGoodwillWith(FactionOwner, goodwill);
            }
        }

        public void RemoveFaction(FactionInteraction faction, AllianceRemoveReason reason)
        {
            Factions.Remove(faction);

            if (reason == AllianceRemoveReason.Kick)
            {
                faction.Faction.TrySetRelationKind(FactionOwner, FactionRelationKind.Hostile);
                faction.Trust -= KickTrustChange;

                foreach (var allianceFaction in Factions)
                    allianceFaction.Faction.TrySetRelationKind(faction.Faction, FactionRelationKind.Hostile);
            }

            if(AllianceAgreements != null)
            {
                for(int i = 0; i < AllianceAgreements.Count; i++)
                {
                    AllianceAgreementComp agreement = AllianceAgreements[i];

                    if(agreement.SignedFaction == faction)
                    {
                        EndAgreement(agreement, AgreementEndReason.FactionLeave);
                    }
                }
            }

            RemoveFromWars(faction);

            Find.LetterStack.ReceiveLetter("AllianceRemoveFactionTitle".Translate(faction.Faction.Name), "Alliance_RemoveFactionDesc".Translate(faction.Faction.Name, reason.Translate()), LetterDefOf.NeutralEvent);
        }

        public void SynchonizeAllOwnerRelations()
        {
            foreach(var allianceFaction in Factions)
            {
                if (allianceFaction.Faction == FactionOwner)
                    continue;

                foreach(var faction in Find.FactionManager.AllFactionsVisible)
                {
                    if (allianceFaction.Faction == faction)
                        continue;

                    if (faction == FactionOwner)
                        continue;

                    var relation = FactionOwner.RelationKindWith(faction);
                    allianceFaction.Faction.TrySetRelationKind(faction, relation);
                }
            }
        }

        public void SynchonizeOwnerRelations(FactionRelationKind syncRelation, Faction syncFaction)
        {
            foreach(var allianceFaction in Factions)
            {
                if (allianceFaction.Faction == FactionOwner)
                    continue;

                if (allianceFaction.Faction == syncFaction)
                    continue;

                allianceFaction.Faction.TrySetRelationKind(syncFaction, syncRelation);
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");

            Scribe_Values.Look(ref PlayerOwner, "PlayerOwner");
            Scribe_References.Look(ref FactionOwner, "FactionOwner");
            Scribe_Defs.Look(ref AllianceGoalDef, "goal");
            Scribe_Collections.Look(ref Factions, "Factions", LookMode.Reference);
            Scribe_Values.Look(ref Name, "Name");
            Scribe_Deep.Look(ref storyState, "storyState", this);
            Scribe_Collections.Look(ref allianceAgreements, "AllianceAgreements", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                InitializeStorytellerComps();
            }
        }

        public string GetUniqueLoadID()
        {
            return "Alliance_" + id;
        }

        public static Alliance MakeAlliance(string name, Faction faction, AllianceGoalDef goal)
        {
            Alliance alliance = new Alliance(name, faction, goal);
            alliance.id = QuestsManager.Communications.UniqueIdManager.GetNextAllianceID();

            return alliance;
        }

        public IEnumerable<IncidentTargetTagDef> IncidentTargetTags()
        {
            yield return IncidentTargetTagDefOfLocal.Alliance;
        }
    }
}
