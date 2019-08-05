using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1
{
    public enum ArmorModuleCategory
    {
        Head,
        Body,
        Legs,
        General
    }

    public class ArmorModuleDef : Def
    {
        public string Icon;

        public Type workerClass;

        public Dictionary<StatDef, float> StatAffecter;

        public float HealRate;

        public ThingDef Item;

        public bool DamageListener = false;

        public bool StatListener = false;

        public bool WornExtraListener = false;

        public ArmorModuleCategory ModuleCategory;

        public int StationLevelRequired = 1;

        public List<ArmorModuleDef> ExcludesModules;

        public Texture2D IconImage
        {
            get
            {
                if (iconImage == null)
                {
                    if (!string.IsNullOrEmpty(Icon))
                    {
                        iconImage = ContentFinder<Texture2D>.Get(Icon);
                    }
                }

                return iconImage;
            }
        }
        private Texture2D iconImage;
    }
}
