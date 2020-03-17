using HarmonyLib;
using RimOverhaul;
using System.Reflection;

namespace ArmorCore
{
    public class ArmorCoreModule : RimOverhaulModule
    {
        public override string ModuleName => "ArmorCore";

        public override void Loaded()
        {
            Harmony harmonyInstance = new Harmony("net.funkyshit.armorcore");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
