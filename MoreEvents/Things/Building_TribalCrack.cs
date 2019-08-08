using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimOverhaul.Things
{
    public class Building_TribalCrack : Building
    {
        public Faction TribalFaction;

        private Lord lord = null;

        public int SpawnRate => 20000;

        private List<Pawn> spawnedMobs = new List<Pawn>();

        private Pawn bomber = null;

        private int blownUp = 90000;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            TribalFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.TribeRough);

            if(TribalFaction == null || TribalFaction.PlayerRelationKind != FactionRelationKind.Hostile)
            {
                Destroy();
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (spawnedMobs.Count < 25)
            {
                if (Find.TickManager.TicksGame % SpawnRate == 0)
                {
                    SpawnMobs();
                }
            }

            if (bomber != null)
            {
                blownUp--;
                if (blownUp <= 0)
                {
                    GoDestroy();
                }
            }
        }

        private void GoDestroy()
        {
            Pawn p = (Pawn)GenSpawn.Spawn(bomber, Position, Map);
            p.SetFaction(Faction.OfPlayer);

            Destroy();

            Find.LetterStack.ReceiveLetter("Building_TribalCrack_Title".Translate(), "Building_TribalCrack_Desc".Translate(bomber.Name.ToStringFull), LetterDefOf.PositiveEvent);
        }

        public void SetBomber(Pawn bomber)
        {
            this.bomber = bomber;

            bomber.DeSpawn();
            Find.WorldPawns.PassToWorld(bomber);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (bomber == null)
            {
                if (CheckPawn(selPawn))
                {
                    yield return new FloatMenuOption("Building_TribalCrack_Option".Translate(), delegate
                    {
                        Job job = new Job(JobDefOfLocal.DestroyTribalCrack, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
                else
                {
                    yield return new FloatMenuOption("Building_TribalCrack_OptionInactive".Translate(), null);
                }
            }
        }

        private bool CheckPawn(Pawn pawn)
        {
            foreach(var item in pawn.inventory.innerContainer)
            {
                if(item.def == ThingDefOfLocal.Weapon_GrenadeFrag)
                {
                    return true;
                }
            }

            foreach (var item in pawn.equipment.AllEquipmentListForReading)
            {
                if (item.def == ThingDefOfLocal.Weapon_GrenadeFrag)
                {
                    return true;
                }
            }

            return false;
        }

        private void SpawnMobs()
        {
            CheckList();

            if (spawnedMobs.Count >= 25)
                return;

            int count = Rand.Range(2, 4);

            if (lord == null)
            {
                lord = LordMaker.MakeNewLord(TribalFaction, new LordJob_AssaultColony(TribalFaction), Map);
            }

            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOfLocal.Tribal_Warrior, TribalFaction, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            for (int i = 0; i < count; i++)
            {
                if (spawnedMobs.Count >= 25)
                    return;

                Pawn pawn = PawnGenerator.GeneratePawn(request);
                spawnedMobs.Add(pawn);
                GenSpawn.Spawn(pawn, this.Position, Map);
                lord.AddPawn(pawn);
            }
        }

        private void CheckList()
        {
            int count = spawnedMobs.Count - 1;
            for (int i = count; i > 0; i--)
            {
                Pawn p = spawnedMobs[i];
                if (p == null || p.Dead)
                    spawnedMobs.RemoveAt(i);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref spawnedMobs, "spawnedMobs", LookMode.Reference);
            Scribe_Values.Look(ref blownUp, "timer");
            Scribe_References.Look(ref bomber, "bomber");
        }
    }
}
