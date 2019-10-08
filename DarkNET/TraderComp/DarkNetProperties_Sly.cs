using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class DarkNetProperties_Sly : DarkNetProperties
    {
        public FloatRange ValueRange;

        public IntRange CountRange;

        public DarkNetProperties_Sly()
        {
            compClass = typeof(DarkNetComp_Sly);
        }
    }
}
