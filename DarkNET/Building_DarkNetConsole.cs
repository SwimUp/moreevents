using DarkNET.Jobs;
using QuestRim.Actions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DarkNET
{
    public class Building_DarkNetConsole: Building
    {
        public CompPowerTrader power;

        public static bool PlayerHasGeoscape = false;

        public bool HasPower
        {
            get
            {
                if (power != null && power.PowerOn)
                {
                    return !this.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                }
                return false;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            power = GetComp<CompPowerTrader>();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (HasPower)
            {
                yield return new FloatMenuOption("OpenCommsDark".Translate(), delegate
                {
                    if (selPawn != null)
                    {
                        Job job = new Job(JobDefOfLocal.UseDarkNET, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    }
                });
            }
            else
            {
                yield return new FloatMenuOption("OpenCommsDarkNoPower".Translate(), null );
            }
        }
        
        public void OpenConsole(Pawn pawn)
        {
            Find.WindowStack.Add(new DarkNETWindow(pawn));
        }
    }
}
