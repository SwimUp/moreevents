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

        public virtual bool CanSign(Alliance alliance, AllianceAgreementDef allianceAgreementDef)
        {
            if (!allianceAgreementDef.TargetGoals.Contains(alliance.AllianceGoalDef))
                return false;

            if (alliance.AgreementActive(allianceAgreementDef))
                return false;

            var conditions = allianceAgreementDef.Conditions;
            if (conditions != null)
            {
                foreach(var condition in conditions)
                {
                    if (!condition.Avaliable(alliance))
                        return false;
                }
            }

            return true;
        }

        public virtual void MenuSelect(Alliance alliance, AllianceAgreementDef allianceAgreementDef)
        {

        }
    }
}
