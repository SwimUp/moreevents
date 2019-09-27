using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimOverhaul.Things
{
    public class Building_LaboratoryConsole : Building
    {
        public bool Used;

        public CompPowerTrader power;

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

            SetFaction(Faction.OfPlayer);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("Building_LaboratoryConsole_UseAction".Translate(), delegate
            {
                Job job = new Job(JobDefOfLocal.UseLaboratoryConsole, this);
                selPawn.jobs.TryTakeOrderedJob(job);
            });
        }

        public void ShowHelp()
        {
            if(!HasPower)
            {
                MakeDialog("Building_LaboratoryConsole_NoPower".Translate());
                return;
            }

            if(Used)
            {
                MakeDialog("Building_LaboratoryConsole_AlreadyUsed".Translate());
            }
        }

        private void MakeDialog(string text)
        {
            DiaNode node = new DiaNode(text);
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Used, "Used");
        }
    }
}
