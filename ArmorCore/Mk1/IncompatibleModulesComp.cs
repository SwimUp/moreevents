using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class IncompatibleModulesComp : ThingComp
    {
        public CompPropertiesIncompatibleModules Props => (CompPropertiesIncompatibleModules)props;

        public List<ArmorModuleDef> ModuleList => Props.ModuleList;
    }
}
