using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasCooler : CompPipe
    {
        public List<PipelineNet> InNet;

        public bool Enabled;

        public float Storage;

        public float MaxStorage => 500;

        public bool Full => Storage == MaxStorage;

        public CompProperties_GasCooler GasCoolerProps => (CompProperties_GasCooler)props;

        public override void NetInit()
        {
            base.NetInit();

            InNet = new List<PipelineNet>();

            var vals = GenAdj.CellsAdjacentCardinal(parent);
            foreach (var cell in vals)
            {
                var thing = cell.GetFirstThing<Building_Pipe>(parent.Map);
                if(thing != null)
                {
                    var compPipe = thing.TryGetComp<CompPipe>();
                    if(compPipe.PipeType == PipeType.NaturalGas && !InNet.Contains(compPipe.pipeNet))
                    {
                        InNet.Add(compPipe.pipeNet);
                    }
                }
            }

            if(pipeNet.GasTankers.Count > 0)
            {
                Enabled = true;
            }
        }

        public void PushGas(GasPlant plant, float count)
        {
            if (Full)
                return;

            Storage += count;
            if(Storage > MaxStorage)
            {
                Storage = MaxStorage;
                count -= Storage - MaxStorage;
            }

            plant.ParentGasWell.GasReserves -= count;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!HasPower)
                return;

            if(Enabled)
            {
                List<CompGasTank> tankers = pipeNet.GasTankers.Where(x => !x.Full).ToList();
                float rate = Mathf.Min(GasCoolerProps.CoolingRate, Storage);
                float toAdd = rate / tankers.Count;
                foreach(var tank in tankers)
                {
                    tank.PushGas(this, toAdd);
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            if (DebugSettings.godMode)
            {
                builder.AppendLine(base.CompInspectStringExtra());
            }
            builder.Append("GasCooler_ActiveInfo".Translate(Storage.ToString("f2"), MaxStorage.ToString("f2"), GasCoolerProps.CoolingRate.ToString("f2")));
            if (pipeNet.GasTankers.Count == 0)
            {
                builder.Append("GasCooler_ActiveInfoNoTankers".Translate());
            }
            if (InNet.Count == 0)
            {
                builder.Append("GasCooler_ActiveInfoNoInNet".Translate());
            }

            return builder.ToString();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref Storage, "Storage");
        }
    }
}
