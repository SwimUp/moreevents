using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class CompPropertiesIncompatibleModules : CompProperties
    {
        public List<ArmorModuleDef> ModuleList;

        public CompPropertiesIncompatibleModules()
        {
            compClass = typeof(IncompatibleModulesComp);
        }
    }
}
