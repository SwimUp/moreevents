using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.CommunicationComps
{
    public class CommunicationComponent_SlyHelp : CommunicationComponent_Timer
    {
        public Faction TraderFaction => Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.DarkNetTraders);

        private List<Pawn> pawns;

        public CommunicationComponent_SlyHelp() : base()
        {

        }

        public CommunicationComponent_SlyHelp(int ticks) : base(ticks)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
        }

        public void GeneratePawns(float points, Dictionary<SkillDef, int> skillsData, List<TraitDef> forceTraits, Faction generateFaction)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = generateFaction,
                points = points,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            foreach(var pawn in pawns)
            {
                if (skillsData != null)
                {
                    foreach (var skillData in skillsData)
                    {
                        pawn.skills.GetSkill(skillData.Key).Level = skillData.Value;
                    }
                }

                if(forceTraits != null)
                {
                    pawn.story.traits.allTraits.Clear();

                    foreach(var newTraitDef in forceTraits)
                    {
                        int degree = 0;
                        if(newTraitDef.degreeDatas != null)
                        {
                            degree = newTraitDef.degreeDatas.RandomElement().degree;
                        }
                        Trait newTrait = new Trait(newTraitDef, degree);
                    }
                }

                pawn.SetFaction(Faction.OfPlayer);
            }
        }

        public void SendPawns(Map map)
        {
            if (pawns == null)
                return;

            IncidentParms parms = new IncidentParms
            {
                target = map,
                spawnCenter = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Walkable(map), map)
        };

            PawnsArrivalModeDefOf.CenterDrop.Worker.Arrive(pawns, parms);
        }

        public override void TimerEnd()
        {
            base.TimerEnd();

            if(pawns != null)
            {
                foreach(var pawn in pawns)
                {
                    if(pawn != null && !pawn.Dead)
                    {
                        pawn.SetFaction(TraderFaction);
                    }
                }
            }
        }
    }
}
