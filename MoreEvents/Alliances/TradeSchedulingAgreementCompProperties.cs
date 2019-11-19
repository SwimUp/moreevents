using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Alliances
{
    public class TradeSchedulingAgreementCompProperties : AllianceAgreementCompProperties
    {
        public TradeSchedulingAgreementCompProperties()
        {
            compClass = typeof(TradeSchedulingAgreementComp);
        }

        public override void MenuSelect(QuestRim.Alliance alliance, AllianceAgreementDef allianceAgreementDef)
        {
            Find.WindowStack.Add(new TradeSchedulingAgreementWindow(alliance, this));
        }
    }
}
