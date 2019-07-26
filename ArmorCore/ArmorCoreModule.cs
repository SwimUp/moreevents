using Harmony;
using RimOverhaul;
using System.Reflection;

namespace ArmorCore
{
    public class ArmorCoreModule : RimOverhaulModule
    {
        public override string ModuleName => "ArmorCore";

        public override void Loaded()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("net.funkyshit.armorcore");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
