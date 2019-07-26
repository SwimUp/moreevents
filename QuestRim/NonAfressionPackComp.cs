using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class NonAfressionPackComp : CommunicationComponent
    {
        public int TicksToPass = 0;

        public Faction Faction;

        public NonAfressionPackComp()
        {

        }

        public NonAfressionPackComp(int days, Faction faction)
        {
            TicksToPass = days * 60000;
            Faction = faction;
        }

        public override void Tick()
        {
            TicksToPass--;

            if(TicksToPass <= 0)
            {
                End();
            }
        }

        public void End()
        {
            FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(Faction);
            foreach (var opt in interaction.Options)
            {
                if (opt is CommOption_NonAgressionPact opt2)
                {
                    opt2.Signed = false;
                }
            }

            Find.LetterStack.ReceiveLetter("NonAfressionPackCompTitle".Translate(), "NonAfressionPackComp".Translate(), LetterDefOf.NeutralEvent);

            QuestsManager.Communications.RemoveComponent(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref TicksToPass, "Ticks");
            Scribe_References.Look(ref Faction, "Faction");
        }
    }
}
