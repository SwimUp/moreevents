﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public enum PipeType : byte
    {
        NaturalGas = 0,
        LiquidGas = 1
    }

    public class CompProperties_GasPipe : CompProperties
    {
        public PipeType pipeType;

        public bool ConnectEverything;

        public bool Transmitter;

        public CompProperties_GasPipe()
        {
            compClass = typeof(CompPipe);
        }
    }
}
