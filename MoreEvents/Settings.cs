﻿using MoreEvents.Events.ShipCrash;
using RimOverhaul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using Verse;

namespace MoreEvents
{
    public class Parameter : IExposable
    {
        public string Param;

        public string Name => Translator.Translate($"{Param}_title");

        public string Description => Translator.Translate($"{Param}");

        public string Value;

        public Parameter()
        {
        }

        public Parameter(string paramName, string value)
        {
            Param = paramName;
            Value = value;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Param, "Param");
            Scribe_Values.Look(ref Value, "Value");
        }
    }

    [XmlRoot("EventSettings")]
    public class EventSettings : IExposable
    {
        [XmlElement("key")]
        public string Key;

        [XmlElement("parameters")]
        public Parameter[] Params;

        [XmlIgnore]
        public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

        [XmlIgnore]
        public string Name => UseCustomLabels == false ? DefDatabase<IncidentDef>.GetNamed(Key).LabelCap : $"{Key}_Title".Translate();

        [XmlIgnore]
        public string Description => UseCustomLabels == false ? DefDatabase<IncidentDef>.GetNamed(Key).letterText : $"{Key}_Desc".Translate().ToString();

        [XmlElement("useCustomLabels")]
        public bool UseCustomLabels = false;

        [XmlIgnore]
        public bool Active = true;

        [XmlElement("requiredModule")]
        public string RequiredModule;

        public EventSettings()
        {

        }

        public EventSettings(string key)
        {
            Key = key;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Active, "EventActive");
            Scribe_Values.Look(ref Key, "Key");
            Scribe_Values.Look(ref UseCustomLabels, "UseCustomLabels");
            Scribe_Collections.Look(ref Parameters, "Parameters", LookMode.Value, LookMode.Deep);
        }

        public void FinalizeSettings()
        {
            if (Params != null)
            {
                Parameters = Params.ToDictionary(k => k.Param);
            }
        }
    }

    public class Settings : ModSettings
    {
        private static Vector2 scroll = Vector2.zero;

        public static string SettingsFileName => "settings.xml";

        #region 1
        public static Dictionary<string, EventSettings> EventsSettings = new Dictionary<string, EventSettings>()
        {
          {
                "General", new EventSettings("General")
                {
                    UseCustomLabels = true,
                    Active = true,
                    Parameters = new Dictionary<string, Parameter>()
                    {
                        {"UseNewMapSizes", new Parameter("UseNewMapSizes", "1") },
                    }
                }
            },
            {
                "ShipCrash", new EventSettings("ShipCrash")
                {
                    Active = true,
                    Parameters = new Dictionary<string, Parameter>()
                    {
                        {"MinParts", new Parameter("MinParts", "4") },
                        {"MaxParts", new Parameter("MaxParts", "10") },
                        {"ShipCargo_Mining_MinSupply", new Parameter("ShipCargo_Mining_MinSupply", "5") },
                        {"ShipCargo_Mining_MaxSupply", new Parameter("ShipCargo_Mining_MaxSupply", "19") },
                        {"ShipCargo_Food_MinSupply", new Parameter("ShipCargo_Food_MinSupply", "5") },
                        {"ShipCargo_Food_MaxSupply", new Parameter("ShipCargo_Food_MaxSupply", "19") },
                        {"ShipCargo_Complex_MinSupply", new Parameter("ShipCargo_Complex_MinSupply", "5") },
                        {"ShipCargo_Complex_MaxSupply", new Parameter("ShipCargo_Complex_MaxSupply", "19") },
                        {"ShipCargo_Armory_MinSupply", new Parameter("ShipCargo_Armory_MinSupply", "5") },
                        {"ShipCargo_Armory_MaxSupply", new Parameter("ShipCargo_Armory_MaxSupply", "12") },
                        {"ShipCargo_Living_MinSupply", new Parameter("ShipCargo_Living_MinSupply", "1") },
                        {"ShipCargo_Living_MaxSupply", new Parameter("ShipCargo_Living_MaxSupply", "5") }
                    }
                }
            },
            {
                "Supernova", new EventSettings("Supernova")
                {
                    Active = true
                }
            },
            {
                "SuperHeatWave", new EventSettings("SuperHeatWave")
                {
                    Active = true
                }
            },
            {
                "MechanoidPortal", new EventSettings("MechanoidPortal")
                {
                    Active = true
                }
            },
            {
                "Disease_NeurofibromatousWorms", new EventSettings("Disease_NeurofibromatousWorms")
                {
                    Active = true
                }
            },
            {
                "Disease_Fibrodysplasia", new EventSettings("Disease_Fibrodysplasia")
                {
                    Active = true
                }
            },
            {
                "DestroyRoad", new EventSettings("DestroyRoad")
                {
                    Active = true
                }
            },
            {
                "BoulderMassHit", new EventSettings("BoulderMassHit")
                {
                    Active = true
                }
            },
            {
                "BeetleRush", new EventSettings("BeetleRush")
                {
                    Active = true
                }
            },
            {
                "RadiationFon", new EventSettings("RadiationFon")
                {
                    Active = true
                }
            },
            {
                "IceStorm", new EventSettings("IceStorm")
                {
                    Active = true
                }
            },
            {
                "HeavyAir", new EventSettings("HeavyAir")
                {
                    Active = true
                }
            },
            {
            "LeanAtmosphere", new EventSettings("LeanAtmosphere")
                {
                    Active = true
                }
            },
            {
            "NoSun", new EventSettings("NoSun")
                {
                    Active = true
                }
            },
            {
            "SandStorm", new EventSettings("SandStorm")
                {
                    Active = true
                }
            },
            {
            "Endlessday", new EventSettings("Endlessday")
                {
                    Active = true
                }
            },
            {
            "DenseAtmosphere", new EventSettings("DenseAtmosphere")
                {
                    Active = true,
                    Parameters = new Dictionary<string, Parameter>()
                    {
                        {"DoMapChange", new Parameter("DoMapChange", "1") },
                    }
                }
            },
            {
            "ClimateBomb", new EventSettings("ClimateBomb")
                {
                    Active = true
                }
            },
            {
            "IonizedAtmosphere", new EventSettings("IonizedAtmosphere")
                {
                    Active = true
                }
            },
            {
            "Earthquake", new EventSettings("Earthquake")
                {
                    Active = true
                }
            },
            {
            "SiegeCamp", new EventSettings("SiegeCamp")
                {
                    Active = true
                }
            },
            {
            "AttackFriendlySettlement", new EventSettings("AttackFriendlySettlement")
                {
                    Active = true
                }
            },
            {
            "Constellations", new EventSettings("Constellations")
                {
                    Active = true,
                    Parameters = new Dictionary<string, Parameter>()
                    {
                        {"EnablePositive", new Parameter("EnablePositive", "1") },
                        {"EnableNegative", new Parameter("EnableNegative", "1") }
                    }
                }
            },
            {
            "MineralMeteorite", new EventSettings("MineralMeteorite")
                {
                    Active = true
                }
            },
            {
            "DropAnimalInsanity", new EventSettings("DropAnimalInsanity")
                {
                    Active = true
                }
            },
            {
            "HungryCannibalRaid", new EventSettings("HungryCannibalRaid")
                {
                    Active = true
                }
            },
            {
            "DoomsdayUltimatum", new EventSettings("DoomsdayUltimatum")
                {
                    Active = true
                }
            },
            {
            "Arsonists", new EventSettings("Arsonists")
                {
                    Active = true
                }
            },
            {
            "UnifiedRaid", new EventSettings("UnifiedRaid")
                {
                    Active = true
                }
            },
            {
            "Quest_ResourceHelp", new EventSettings("Quest_ResourceHelp")
                {
                    Active = true
                }
            },
            {
            "Quest_BuildNewBase", new EventSettings("Quest_BuildNewBase")
                {
                    Active = true
                }
            },
            {
            "Quest_MissingPeople", new EventSettings("Quest_MissingPeople")
                {
                    Active = true
                }
            },
            {
            "Psychogas", new EventSettings("Psychogas")
                {
                    Active = true
                }
            },
            {
            "Disease_ZeroMechanites", new EventSettings("Disease_ZeroMechanites")
                {
                    Active = true
                }
            },
            {
            "SummerSolstice", new EventSettings("SummerSolstice")
                {
                    Active = true
            }
            },
            {
            "Pocahontas", new EventSettings("Pocahontas")
                {
                    Active = true
            }
            },
            {
            "SeaAir", new EventSettings("SeaAir")
                {
                    Active = true
            }
            },
            {
            "MountainAir", new EventSettings("MountainAir")
                {
                    Active = true
            }
            },
            {
            "Quest_KillLeader", new EventSettings("Quest_KillLeader")
            {
                    Active = true
            }
            },
            {
            "Quest_SuppressionRebellion", new EventSettings("Quest_SuppressionRebellion")
            {
                    Active = true
            }
            },
            {
            "SpaceBattle", new EventSettings("SpaceBattle")
            {
                    Active = true
            }
            },
            {
            "Quest_KillOrder", new EventSettings("Quest_KillOrder")
            {
                    Active = true
            }
            },
            {
            "ConcantrationCamp", new EventSettings("ConcantrationCamp")
            {
                    Active = true
            }
            },
            {
            "TunnelRats", new EventSettings("TunnelRats")
            {
                    Active = true
            }
            },
            {
            "RaidEnemyWithAnimals", new EventSettings("RaidEnemyWithAnimals")
            {
                    Active = true,
                    UseCustomLabels = true
            }
            },
            {
            "HighMutantPopulation", new EventSettings("HighMutantPopulation")
            {
                    Active = true
            }
            },
            {
            "Fair", new EventSettings("Fair")
            {
                    Active = true
            }
            },
            {
            "ExplosiveFever", new EventSettings("ExplosiveFever")
            {
                    Active = true
            }
            },
            {
            "GlacialPeriod", new EventSettings("GlacialPeriod")
            {
                    Active = true
            }
            },
            {
            "ActiveStar", new EventSettings("ActiveStar")
            {
                    Active = true
            }
            },
            {
            "Competitions", new EventSettings("Competitions")
            {
                    Active = true
            }
            },
            {
            "MassBurial", new EventSettings("MassBurial")
            {
                    Active = true
            }
            },
            {
            "PlaceBattle", new EventSettings("PlaceBattle")
            {
                    Active = true
            }
            },
            {
            "AmbushTwoFactions", new EventSettings("AmbushTwoFactions")
            {
                    Active = true
            }
            },
            {
            "PrisonShipAccident", new EventSettings("PrisonShipAccident")
            {
                    Active = true
            }
            }
        };
        
        #endregion
        //public static Dictionary<string, EventSettings> EventsSettings = new Dictionary<string, EventSettings>();

        private static int length = 0;

        public string SettingsPath => Path.Combine(RootDir, SettingsFileName);

        public static string RootDir;

        private List<EventSettings> fileSettings;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            /*
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.GapLine();
            Rect mainScrollVertRect = new Rect(0, 0, inRect.x, length);
            listing_Standard.BeginScrollView(inRect, ref scroll, ref mainScrollVertRect);
            listing_Standard.Label(Translator.Translate("MEM_Settings_General"));
            listing_Standard.GapLine();
            foreach (var setting in EventsSettings)
            {
                try
                {
                    listing_Standard.Label(setting.Value.Name, tooltip: setting.Value.Description);
                    Rect rect2 = new Rect(0, listing_Standard.CurHeight, 600, 20);
                    if (listing_Standard.RadioButton(Translator.Translate("EventActive"), setting.Value.Active))
                    {
                        setting.Value.Active = !setting.Value.Active;
                    }

                    foreach (var param in setting.Value.Parameters)
                    {
                        Rect rect3 = new Rect(0, listing_Standard.CurHeight, 600, 20);
                        TooltipHandler.TipRegion(rect3, param.Value.Description);
                        param.Value.Value = listing_Standard.TextEntryLabeled(param.Value.Name, param.Value.Value.ToString());
                    }

                    listing_Standard.GapLine();
                }catch
                {
                    Log.Message($"Error --> {setting.Key}");
                }
            }
            listing_Standard.EndScrollView(ref mainScrollVertRect);

            listing_Standard.End();
            */
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref EventsSettings, "EventsSettings", LookMode.Value, LookMode.Deep);

            CheckSettings();
        }

        public void CheckSettings()
        {
            if (fileSettings == null)
            {
                try
                {
                    fileSettings = XmlUtility.Deserialize<List<EventSettings>>(SettingsPath);
                }catch(Exception ex)
                {
                    Log.Error("Error while deserialize settings file " + ex);
                }
            }

            bool CanHoldSetting(EventSettings fSetting)
            {
                return !(!string.IsNullOrEmpty(fSetting.RequiredModule) && ModulesHandler.GetModule(fSetting.RequiredModule) == null);
            }

            if (fileSettings != null)
            {
                if (EventsSettings == null)
                {
                    EventsSettings = new Dictionary<string, EventSettings>();
                }

                foreach (var fSetting in fileSettings)
                {
                    bool canHoldSetting = CanHoldSetting(fSetting);
                    if (!EventsSettings.TryGetValue(fSetting.Key, out EventSettings value))
                    {
                        if (!canHoldSetting)
                            continue;

                        Log.Message($"New settings {fSetting.Key} found, added");

                        fSetting.FinalizeSettings();

                        EventsSettings.Add(fSetting.Key, fSetting);
                    }
                    else
                    {
                        if (!canHoldSetting)
                        {
                            EventsSettings.Remove(fSetting.Key);
                            Log.Message($"Settings {fSetting.Key} will be deleted (the required module is missing)");
                        }
                        else if(fSetting.Params != null && value.Parameters.Count != fSetting.Params.Length)
                        {
                            value.Params = fSetting.Params;

                            value.FinalizeSettings();
                        }
                    }
                }

                length = EventsSettings.Count * 60;
                foreach (var setting in EventsSettings)
                {
                    foreach (var param in setting.Value.Parameters)
                    {
                        length += 30;
                    }
                }
            }
        }
    }
}