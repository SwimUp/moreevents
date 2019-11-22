using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.AI
{
    public class CaravanAI : Caravan
    {
        public bool ShowNeeds = false;

        public bool ShowSocial = false;

        public bool UseFood = false;

        public CaravanAI_NeedsTracker aiNeeds;

        public CaravanAI() : base()
        {
            aiNeeds = new CaravanAI_NeedsTracker(this);
        }

        public override void Tick()
        {
            for (int i = 0; i < AllComps.Count; i++)
            {
                AllComps[i].CompTick();
            }

            CheckAnyNonWorldPawns();
            pather.PatherTick();
            tweener.TweenerTick();
            forage.ForageTrackerTick();
            carryTracker.CarryTrackerTick();
            beds.BedsTrackerTick();
            aiNeeds.NeedsTrackerTick();
            CaravanDrugPolicyUtility.CheckTakeScheduledDrugs(this);
            CaravanTendUtility.CheckTend(this);
        }

        public override IEnumerable<InspectTabBase> GetInspectTabs()
        {
            if (def.inspectorTabsResolved != null)
            {
                foreach(var tab in def.inspectorTabsResolved)
                {
                    if (!ShowNeeds && tab is WITab_Caravan_Needs)
                        continue;

                    if (!ShowSocial && tab is WITab_Caravan_Social)
                        continue;

                    yield return tab;
                }
            }

            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ShowNeeds, "ShowPawns");
            Scribe_Values.Look(ref ShowSocial, "ShowSocial");
            Scribe_Values.Look(ref UseFood, "UseFood");
            Scribe_Deep.Look(ref aiNeeds, "aiNeeds", this);
        }

        private void CheckAnyNonWorldPawns()
        {
            for (int num = pawns.Count - 1; num >= 0; num--)
            {
                if (!pawns[num].IsWorldPawn())
                {
                    Log.Error("Caravan member " + pawns[num] + " is not a world pawn. Removing...");
                    pawns.Remove(pawns[num]);
                }
            }
        }
    }
}
