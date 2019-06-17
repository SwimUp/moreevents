using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents
{
    [StaticConstructorOnStartup]
    public class MoreEventsMod : Mod
    {
        internal static HarmonyInstance harmonyInstance;

        public static Settings Settings;

        public MoreEventsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();

            harmonyInstance = HarmonyInstance.Create("net.funkyshit.moreeventsmod");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory()
        {
            return "MoreEventsMod";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
