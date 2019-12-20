using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Alliances
{
    public class DefenseContractCompProperties : AllianceAgreementCompProperties
    {
        public TechLevel MinFactionTechLevel;
        public int TrustCostPerDay;
        public Dictionary<FightersLevel, int> FightersCost;

        public DefenseContractCompProperties()
        {
            compClass = typeof(DefenseContractComp);
        }

        public override bool CanSign(Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn speaker, out string reason)
        {
            if (!base.CanSign(alliance, allianceAgreementDef, speaker, out reason))
                return false;

            if((int)speaker.Faction?.def.techLevel < (int)MinFactionTechLevel)
            {
                reason = "DefenseContractCompProperties_FactionTechLevelLack".Translate(speaker.Faction.Name, speaker.Faction.def.techLevel.ToStringHuman(), MinFactionTechLevel.ToStringHuman());
                return false;
            }

            return true;
        }

        public override void MenuSelect(QuestRim.Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn negotiator)
        {
            Find.WindowStack.Add(new DefenseContractCompWindow(alliance, this, negotiator));
        }
    }
}
