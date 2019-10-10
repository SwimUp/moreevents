using DarkNET.CommunicationComps;
using DarkNET.Traders;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Sly
{
    public class SlyService_RaidHelp : SlyHelp_OneUse
    {
        public override string Label => "SlyService_RaidHelp_Label".Translate();

        public override string Description => "SlyService_RaidHelp_Description".Translate(CostRaid_Pacifists, CostRaid_NormalHelp, CostRaid_Guardians);

        private int CostRaid_Pacifists => 700;

        private int CostRaid_NormalHelp => 1200;

        private int CostRaid_Guardians => 1800;

        public Dictionary<SkillDef, int> PacifistsSkills => new Dictionary<SkillDef, int>
        {
            {SkillDefOf.Medicine, Rand.Range(10, 20) },
            {SkillDefOf.Shooting, 0 },
            {SkillDefOf.Melee, 0 },
            {SkillDefOf.Cooking, Rand.Range(10, 20) }
        };

        public Dictionary<SkillDef, int> NormalSkills => new Dictionary<SkillDef, int>
        {
            {SkillDefOf.Shooting, Rand.Range(4, 10) },
            {SkillDefOf.Melee, Rand.Range(4, 10) },
        };

        public Dictionary<SkillDef, int> GuardiansSkills => new Dictionary<SkillDef, int>
        {
            {SkillDefOf.Shooting, Rand.Range(13, 20) },
            {SkillDefOf.Melee, Rand.Range(13, 20) },
        };

        public List<TraitDef> GuardiansTratis => new List<TraitDef>
        {
            TraitDefOf.ShootingAccuracy,
            TraitDefOf.SpeedOffset
        };

        private float randomAvaliableChance => 0.2f;
        private bool avaliable;

        public override IEnumerable<FloatMenuOption> Options(Map map)
        {
            yield return new FloatMenuOption("SlyService_RaidHelp_Pacifists".Translate(), delegate
            {
                GenerateAndSendPawns(CostRaid_Pacifists, map, Rand.Range(60, 150), PacifistsSkills, new List<TraitDef>(), TryResolveFaction());
            });
            yield return new FloatMenuOption("SlyService_RaidHelp_NormalHelp".Translate(), delegate
            {
                GenerateAndSendPawns(CostRaid_NormalHelp, map, Rand.Range(100, 200), NormalSkills, new List<TraitDef>(), TryResolveFaction());
            });
            yield return new FloatMenuOption("SlyService_RaidHelp_Guardians".Translate(), delegate
            {
                GenerateAndSendPawns(CostRaid_Guardians, map, Rand.Range(150, 300), GuardiansSkills, GuardiansTratis, TryResolveFaction());
            });
        }

        public override bool AvaliableRightNow(out string reason)
        {
            if(!avaliable)
            {
                reason = Rand.Chance(50) ? "SlyService_RaidHelp_NonAvaliable_ver1".Translate() : "SlyService_RaidHelp_NonAvaliable_ver2".Translate();
                return false;
            }

            return base.AvaliableRightNow(out reason);
        }

        public override void SlyArrival(TraderWorker_Sly sly)
        {
            base.SlyArrival(sly);

            avaliable = !Rand.Chance(randomAvaliableChance);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref avaliable, "avaliable");
        }

        private Faction TryResolveFaction()
        {
            return Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Pirate);
        }

        private void GenerateAndSendPawns(int price, Map map, float points, Dictionary<SkillDef, int> skillsData, List<TraitDef> forceTraits, Faction generateFaction)
        {
            if (!DarkNetPriceUtils.TakeSilverFromPlayer(price, map))
                return;

            CommunicationComponent_SlyHelp slyHelp = new CommunicationComponent_SlyHelp(2 * 60000);
            slyHelp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();

            slyHelp.GeneratePawns(points, skillsData, forceTraits, generateFaction);

            slyHelp.SendPawns(map);

            QuestsManager.Communications.RegisterComponent(slyHelp);

            alreadyUsed = true;

            Find.LetterStack.ReceiveLetter("SlyService_RaidHelp_HelpSendedTitle".Translate(), "SlyService_RaidHelp_HelpSendedDesc".Translate(), LetterDefOf.PositiveEvent);
        }
    }
}
