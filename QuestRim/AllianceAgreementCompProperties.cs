using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class AllianceAgreementCompProperties
    {
        [TranslationHandle]
        public Type compClass;

        public AllianceAgreementCompProperties()
        {
        }

        public AllianceAgreementCompProperties(Type compClass)
        {
            this.compClass = compClass;
        }

        public virtual bool CanSign(Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn speaker, out string reason)
        {
            reason = string.Empty;

            if (alliance.AgreementActive(allianceAgreementDef))
            {
                reason = "Alliance_CanSignAgreement_AlreadyActive".Translate();
                return false;
            }

            if (!allianceAgreementDef.TargetGoals.Contains(alliance.AllianceGoalDef))
            {
                reason = "Alliance_CanSignAgreement_NoTargetGoal".Translate();
                return false;
            }

            if (allianceAgreementDef.UseAgreementsSlot && alliance.AllianceAgreements.Count == alliance.AgreementsSlots)
            {
                reason = "Alliance_CanSignAgreement_NoSlots".Translate(alliance.AgreementsSlots);
                return false;
            }

            var conditions = allianceAgreementDef.Conditions;
            if (conditions != null)
            {
                foreach(var condition in conditions)
                {
                    if (!condition.Avaliable(alliance))
                    {
                        reason = condition.Reason;
                        return false;
                    }
                }
            }

            if(alliance.Factions.Count < allianceAgreementDef.MinMembersRequired)
            {
                reason = "Alliance_CanSignAgreement_MinMembersRequired".Translate(allianceAgreementDef.MinMembersRequired);
                return false;
            }

            return true;
        }

        public virtual void MenuSelect(Alliance alliance, AllianceAgreementDef allianceAgreementDef, Pawn negotiator)
        {

        }
    }
}
