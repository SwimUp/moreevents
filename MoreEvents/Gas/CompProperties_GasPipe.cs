using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public enum PipeType : byte
    {
        NaturalGas,
        LiquidGas
    }

    public class CompProperties_GasPipe : CompProperties
    {
        public PipeType pipeType;

        public bool ConnectEverything;

        public CompProperties_GasPipe()
        {
            compClass = typeof(CompPipe);
        }
    }
}
