using MapGenerator;
using MoreEvents.MapGeneratorFactionBase;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class FriendlySettlementComp : WorldObjectComp
    {
        public int TicksToAttack = 0;
        public bool Enable;
        public float DaysToAttack => GenDate.TicksToDays(TicksToAttack);

        public FriendlySettlement settlement;

        private List<Pawn> friendlyPawns = new List<Pawn>();
        private int friendlyPawnsBeforeAttack;
        private List<Pawn> enemyPawns = new List<Pawn>();
        public bool ShowThreat = false;
        public float Points = 0f;
        public Faction OffensiveFaction;

        public bool HelpShowed = false;

        private int checkEnemyTicker = 0;
        private int checkEnemyInterval = 5000;
        private bool attackStarted => !Enable && !settlement.AttackRepelled && enemyPawns.Count > 0;
        public readonly FloatRange RewardMarketValueRange = new FloatRange(150, 600);

        private string blueprintsPattern => $"FriendlyFBByLevel_{(int)settlement.Faction.def.techLevel}";

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);

            settlement = (FriendlySettlement)parent;
            Enable = true;
        }

        public void InitPoints()
        {
            Points = Rand.Range(300, 800) * (int)OffensiveFaction.def.techLevel;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref TicksToAttack, "TicksToAttack");
            Scribe_Values.Look(ref Enable, "Enable");
            Scribe_References.Look(ref OffensiveFaction, "offensiveFaction");
            Scribe_Values.Look(ref Points, "points");
            Scribe_Collections.Look(ref enemyPawns, "enemyPawns", LookMode.Reference);
            Scribe_Collections.Look(ref friendlyPawns, "friendlyPawns", LookMode.Reference);

            friendlyPawnsBeforeAttack = friendlyPawns.Count;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("TimerDaysToAttack".Translate(DaysToAttack.ToString("f2")));
            if(ShowThreat)
                builder.Append("ThreatLevel".Translate(Points));

            return builder.ToString();
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            BaseBlueprintDef basePrint = GetBaseBlueprint();

            BlueprintHandler.CreateBlueprintAt(settlement.Map.Center, settlement.Map, basePrint, settlement.Faction, null, out Dictionary<Pawn, LordType> pawns, out float totalThreat, true);
            foreach (var p in pawns.Keys)
            {
                friendlyPawns.Add(p);
                if (p != null)
                {
                    p.SetFaction(Faction.OfPlayer);
                }
            }

            friendlyPawnsBeforeAttack = friendlyPawns.Count;

            if (HelpShowed == false)
            {
                ShowHelp();
            }
        }

        private BaseBlueprintDef GetBaseBlueprint()
        {
            BaseBlueprintDef print = DefDatabase<BaseBlueprintDef>.AllDefs.Where(b => b.Categories != null && b.Categories.Contains(blueprintsPattern)).RandomElement();

            if (print == null)
                print = BlueprintDefOfLocal.SiegeCampBase2_1;

            return print;
        }

        private void ShowHelp()
        {
            HelpShowed = true;

            DiaNode node = new DiaNode("AttackedFriendlySettlementDefendHelp".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (Enable)
            {
                TicksToAttack--;

                if (TicksToAttack <= 0)
                {
                    AttackSetltmenetNow();
                }
            }

            if(attackStarted)
            {
                checkEnemyTicker--;

                if(checkEnemyTicker <= 0)
                {
                    checkEnemyTicker = checkEnemyInterval;

                    CheckEnemiesNow();
                }
            }
        }

        public void CheckEnemiesNow()
        {
            int dead = 0;
            foreach(var p in enemyPawns)
            {
                if (p == null || p.Dead || p.Downed || !p.Spawned)
                    dead++;
            }

            if(dead == enemyPawns.Count)
            {
                DoVictory();
            }
        }

        public void DoVictory()
        {
            settlement.AttackRepelled = true;
            TransferColonists(false);

            int savedPawns = friendlyPawnsBeforeAttack - friendlyPawns.Where(p => p != null && p.Dead).Count();
            if (savedPawns <= 0)
            {
                DoLose(true);
                return;
            }

            settlement.Quest.EndQuest(null, QuestRim.EndCondition.Success);

            settlement.Faction.TryAffectGoodwillWith(Faction.OfPlayer, 2 + savedPawns);

            FloatRange value = (RewardMarketValueRange * (int)settlement.Faction.def.techLevel);
            int pawnsValue = savedPawns * 50;
            value.min += pawnsValue;
            value.max += pawnsValue;
            List<Thing> rewards = GenerateRewards(value);
            StringBuilder builder = new StringBuilder();
            foreach (var thing in rewards)
            {
                builder.Append($"\n- {thing.LabelCap}");
                GenDrop.TryDropSpawn(thing, settlement.Map.Center, settlement.Map, ThingPlaceMode.Near, out Thing t);
            }
            DiaNode node = new DiaNode("AttackedFriendlySettlementDefendVictory".Translate(settlement.Faction, savedPawns, builder.ToString()));
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        private List<Thing> GenerateRewards(FloatRange totalValue)
        {
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.countRange = new IntRange(2, 10);
            parms.totalMarketValueRange = totalValue;
            return ThingSetMakerDefOf.Reward_ItemStashQuestContents.root.Generate(parms);
        }

        public void TransferColonists(bool toPlayer)
        {
            if(toPlayer)
            {
                foreach(var p in friendlyPawns)
                {
                    if (p != null && !p.Dead)
                    {
                        p.SetFaction(Faction.OfPlayer);
                        p.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
                    }
                }
            }
            else
            {
                Lord lord = LordMaker.MakeNewLord(settlement.Faction, new LordJob_DefendBase(settlement.Faction, settlement.Map.Center), settlement.Map);

                foreach (var p in friendlyPawns)
                {
                    if (p != null)
                    {
                        p.SetFaction(settlement.Faction);
                        
                        if(!p.Dead && !p.Downed)
                            lord.AddPawn(p);
                    }
                }
            }
        }

        public void DoLose(bool anyHelp = false)
        {
            settlement.Quest.EndQuest(null, QuestRim.EndCondition.Fail);
            bool defeated = true;
            DiaNode node = null;
            if (anyHelp)
            {
                node = new DiaNode("AttackedFriendlySettlementDefendLoseWithHelp".Translate());
            }
            else
            {
                node = new DiaNode("AttackedFriendlySettlementLoseWithoutHelp".Translate());

                float liveChance = 0.20f * (int)settlement.Faction.def.techLevel;
                if(liveChance > Rand.Value)
                {
                    defeated = false;
                }

                Find.WorldObjects.Remove(settlement);
            }

            if (defeated)
            {
                var factionSettlement = Find.WorldObjects.SettlementAt(settlement.Tile);
                if (factionSettlement != null)
                {
                    Find.WorldObjects.Remove(factionSettlement);

                    if (!HasAnySettlement(settlement.Faction))
                    {
                        settlement.Faction.defeated = true;
                    }
                }

                if(!settlement.Faction.defeated)
                {
                    settlement.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -(5 * (int)OffensiveFaction?.def.techLevel));
                }
            }
            else
            {
                if (!settlement.Faction.defeated)
                {
                    settlement.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -(5 * (int)OffensiveFaction?.def.techLevel));
                }
            }

            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        private bool HasAnySettlement(Faction faction)
        {
            foreach (var sett in Find.WorldObjects.Settlements)
            {
                if (sett.Faction == faction)
                {
                    return true;
                }
            }

            return false;
        }

        public void AttackSetltmenetNow()
        {
            Enable = false;
            TicksToAttack = 0;
            if (settlement.HasMap)
            {
                PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
                {
                    faction = OffensiveFaction,
                    points = Points,
                    generateFightersOnly = true,
                    groupKind = PawnGroupKindDefOf.Combat,
                    raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                    forceOneIncap = true
                };

                TryFindSpawnSpot(settlement.Map, out IntVec3 spawnSpot);

                enemyPawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
                foreach(var p in enemyPawns)
                {
                    GenSpawn.Spawn(p, spawnSpot, settlement.Map);
                }

                LordJob lordJob = new LordJob_AssaultColony(parent.Faction, canKidnap: true, canTimeoutOrFlee: false);
                if (lordJob != null)
                {
                    Lord lord = LordMaker.MakeNewLord(parent.Faction, lordJob, settlement.Map, enemyPawns);
                    lord.numPawnsLostViolently = int.MaxValue;
                }

                Find.LetterStack.ReceiveLetter("RaidStartDefendTitle".Translate(), "RaidStartDefend".Translate(), LetterDefOf.NeutralEvent, new LookTargets(enemyPawns));
            }
            else
            {
                DoLose();
            }
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }
    }
}
