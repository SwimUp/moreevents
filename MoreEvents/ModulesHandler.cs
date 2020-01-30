using RimOverhaul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace MoreEvents
{
    public static class ModulesHandler
    {
        public static Dictionary<Type, RimOverhaulModule> Modules;
        public static List<RimOverhaulModule> ModulesList => Modules?.Values.ToList();

        public static void TryInjectModules()
        {
            Log.Message("Trying inject modules...");

            Modules = new Dictionary<Type, RimOverhaulModule>();

            foreach (Type type in typeof(RimOverhaulModule).InstantiableDescendantsAndSelf())
            {
                try
                {
                    if(!Modules.ContainsKey(type))
                    {
                        Modules[type] = (RimOverhaulModule)Activator.CreateInstance(type);
                        Modules[type].Loaded();

                        Log.Message($"[RimOevrhaul] Module {Modules[type].ModuleName} loaded");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[RimOverhaul] Eror while loading module: {type} --> {ex}");
                }
            }

            Log.Message("Injecting end...");
        }

        public static RimOverhaulModule GetModule(string moduleName)
        {
            if (ModulesList == null)
                return null;

            foreach(var module in ModulesList)
            {
                if (module.ModuleName == moduleName)
                    return module;
            }

            return null;
        }
    }
}
