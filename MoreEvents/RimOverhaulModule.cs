using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RimOverhaul
{
    public abstract class RimOverhaulModule
    {
        public abstract string ModuleName { get; }

        public virtual void Loaded()
        {

        }
    }
}
