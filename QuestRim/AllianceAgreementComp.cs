using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class AllianceAgreementComp : IExposable
    {
        public AllianceAgreementCompProperties props => AllianceAgreementDef.Comp;

        public AllianceAgreementDef AllianceAgreementDef;

        public FactionInteraction SignedFaction;

        public FactionInteraction OwnerFaction;

        public Alliance Alliance;

        public int SignTicks;

        public int EndTicks;

        public virtual bool Passed => Find.TickManager.TicksGame >= EndTicks;

        public virtual void MenuSelect()
        {

        }

        public virtual void Tick()
        {
            if(Passed)
            {
                Alliance.EndAgreement(this);
            }
        }

        public virtual void End()
        {

        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref AllianceAgreementDef, "AllianceAgreementDef");
            Scribe_References.Look(ref SignedFaction, "SignedFaction");
            Scribe_References.Look(ref OwnerFaction, "OwnerFaction");
            Scribe_References.Look(ref Alliance, "Alliance");

            Scribe_Values.Look(ref SignTicks, "SignTicks");
            Scribe_Values.Look(ref EndTicks, "EndTicks");
        }
    }
}
