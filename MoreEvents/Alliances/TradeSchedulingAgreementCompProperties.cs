using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Alliances
{
    public class TradeSchedulingAgreementCompProperties : AllianceAgreementCompProperties
    {
        public float multiplierPerItemMass;
        public float prepareMultiplierPerItem;
        public int trustCost;
        public float baseDiscount;
        public int maxAdditionalTrust;
        public float maxDiscount;
        public float discountPerAdditionalTrust;

        public TradeSchedulingAgreementCompProperties()
        {
            compClass = typeof(TradeSchedulingAgreementComp);
        }

        public override bool CanSign(Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn speaker, out string reason)
        {
            if (!base.CanSign(alliance, allianceAgreementDef, speaker, out reason))
                return false;

            if(speaker.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
            {
                reason = "TradeSchedulingAgreementCompProperties_SkilLDisabled".Translate();
                return false;
            }

            return true;
        }

        public override void MenuSelect(QuestRim.Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn negotiator)
        {
            Find.WindowStack.Add(new TradeSchedulingAgreementWindow(alliance, this, negotiator));
        }
    }
}
