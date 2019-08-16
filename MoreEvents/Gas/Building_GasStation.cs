using RimOverhaul.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class Building_GasStation : Building, IBillGiver, IBillGiverWithTickAction
    {
        public CompGasStation CompGasStation;

        public BillStack billStack;

        private CompPowerTrader powerComp;

        private CompRefuelable refuelableComp;

        private CompBreakdownable breakdownableComp;

        public bool CanWorkWithoutPower
        {
            get
            {
                if (powerComp == null)
                {
                    return true;
                }
                if (def.building.unpoweredWorkTableWorkSpeedFactor > 0f)
                {
                    return true;
                }
                return false;
            }
        }

        public BillStack BillStack => billStack;

        public IntVec3 BillInteractionCell => InteractionCell;

        public IEnumerable<IntVec3> IngredientStackCells => GenAdj.CellsOccupiedBy(this);

        public PipelineNet CurrentNet;

        public List<CompGasTank> NetTankers;

        public Dictionary<RecipeDef, float> GasModifiers;

        public float Storage;

        public float MaxStorage => 500;

        public float PullRate => 2.2f;

        public Building_GasStation()
        {
            billStack = new BillStack(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref billStack, "billStack", this);
            Scribe_Values.Look(ref Storage, "Storage");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            refuelableComp = GetComp<CompRefuelable>();
            breakdownableComp = GetComp<CompBreakdownable>();
            foreach (Bill item in billStack)
            {
                item.ValidateSettings();
            }
        }

        public void UsedThisTick()
        {
            if (refuelableComp != null)
            {
                refuelableComp.Notify_UsedThisTick();
            }

            Storage = Mathf.Clamp(Storage - GasModifiers[BillStack.FirstShouldDoNow.recipe], 0, Storage);
        }

        public override void Tick()
        {
            if (this.IsHashIntervalTick(150))
            {
                if (CurrentNet == null || NetTankers.Count == 0)
                    return;

                if (Storage == MaxStorage)
                    return;

                float toPull = Mathf.Min(PullRate, NetTankers.Sum(x => x.Storage));
                Storage += toPull;
                if (Storage > MaxStorage)
                {
                    Storage = MaxStorage;
                    toPull -= Storage - MaxStorage;
                }

                CurrentNet.GetGasFromNet(toPull);
            }
        }

        public bool CurrentlyUsableForBills()
        {
            if (!UsableForBillsAfterFueling())
            {
                return false;
            }
            if (!CanWorkWithoutPower && (powerComp == null || !powerComp.PowerOn))
            {
                return false;
            }

            return true;
        }

        public override string GetInspectString()
        {
            return $"{base.GetInspectString()}\n{"GasStation_Info".Translate(Storage.ToString("f2"), MaxStorage.ToString("f2"))}";
        }

        public bool UsableForBillsAfterFueling()
        {
            if (!CanWorkWithoutPower && (powerComp == null || !powerComp.PowerOn))
            {
                return false;
            }
            if (breakdownableComp != null && breakdownableComp.BrokenDown)
            {
                return false;
            }

            return true;
        }

        public void Reload()
        {
            if (CompGasStation == null)
            {
                CompGasStation = GetComp<CompGasStation>();
                GasModifiers = CompGasStation.Props.GasModifier;
            }

            CurrentNet = CompGasStation.pipeNet;
            NetTankers = CurrentNet.GasTankers;
        }
    }
}
