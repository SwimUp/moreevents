using HarmonyLib;
using RimWorld;
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
        //internal static HarmonyInstance harmonyInstance --> OLD
        internal static Harmony harmonyInstance; //--NEW

        public static Settings Settings;

        public static EventSettings GeneralSettings => Settings.EventsSettings["General"];

        public MoreEventsMod(ModContentPack content) : base(content)
        {
            Settings.RootDir = content.RootDir;
            ModulesHandler.TryInjectModules();

            Settings = GetSettings<Settings>();

            //harmonyInstance = HarmonyInstance.Create("net.funkyshit.moreeventsmod"); --> OLD
            harmonyInstance = new Harmony("net.funkyshit.moreeventsmod"); //--> NEW
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            //if(!Settings.EventsSettings.ContainsKey("General"))
            //    Settings.CheckSettings();

            int useNewMapSizes = int.Parse(GeneralSettings.Parameters["UseNewMapSizes"].Value);
            if (useNewMapSizes == 1)
            {
                Type type = typeof(Dialog_AdvancedGameConfig);
                FieldInfo field = type.GetField("MapSizes", BindingFlags.NonPublic | BindingFlags.Static);

                int[] newSizes = new int[] { 25, 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325 };

                field.SetValue(null, newSizes);
            }
        }

        public override string SettingsCategory()
        {
            return "RimOverhaul";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
