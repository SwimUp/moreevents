using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class DoomsdayUltimatumComp : WorldObjectComp
    {
        public int Timer;

        public DoomsdaySite Parent;

        public DoomsdayUltimatumComp()
        {
            Parent = (DoomsdaySite)parent;
        }

        public void SetTimer(int days) => Timer = days * 60000;

        public override void CompTick()
        {
            base.CompTick();

            Timer--;

            if (Timer <= 0)
            {
                //do KAVABANGA
            }

        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref Timer, "Timer");
        }

        public override string CompInspectStringExtra()
        {
            string result = $"{Translator.Translate("PlanetWillBeDestroyed")}{GenDate.TicksToDays(Timer).ToString("f2")}";

            return result;
        }
    }
}
