using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents
{
    [StaticConstructorOnStartup]
    public class MoreEventsMod : Mod
    {
        public static Settings Settings;

        public MoreEventsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
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
