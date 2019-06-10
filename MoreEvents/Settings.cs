using MoreEvents.Events.ShipCrash;
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

    public class EventSettings : IExposable
    {
        public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

        public string Key;

        public string Name => Translator.Translate($"{Key}_Title");

        public string Description => Translator.Translate($"{Key}_Desc");

        public bool Active = true;

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
            Scribe_Collections.Look(ref Parameters, "Parameters", LookMode.Value, LookMode.Deep);
        }
    }

    public class Settings : ModSettings
    {
        private static Vector2 scroll = Vector2.zero;

        private static int totalSettings = 19;

        public static Dictionary<string, EventSettings> EventsSettings = new Dictionary<string, EventSettings>()
        {
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
                "Disease_NeurofibromatousWorm", new EventSettings("Disease_NeurofibromatousWorm")
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
                "SandStorm", new EventSettings("SandStorm")
                {
                    Active = true
                }
            },
            {
            "MassiveFire", new EventSettings("MassiveFire")
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
                    Active = true
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
            }
        };

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.GapLine();
            Rect mainScrollVertRect = new Rect(0, 0, inRect.x, 1480);
            listing_Standard.BeginScrollView(inRect, ref scroll, ref mainScrollVertRect);
            listing_Standard.Label(Translator.Translate("MEM_Settings_General"));
            listing_Standard.GapLine();
            foreach (var setting in EventsSettings)
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
            }
            listing_Standard.EndScrollView(ref mainScrollVertRect);

            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref EventsSettings, "EventsSettings", LookMode.Value, LookMode.Deep);

            if (EventsSettings.Count != totalSettings)
            {
                ReloadSettings();
            }
        }

        private void ReloadSettings()
        {
            EventsSettings = new Dictionary<string, EventSettings>()
            {
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
                    "Disease_NeurofibromatousWorm", new EventSettings("Disease_NeurofibromatousWorm")
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
                "SandStorm", new EventSettings("SandStorm")
                {
                    Active = true
                }
            },
            {
            "MassiveFire", new EventSettings("MassiveFire")
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
                    Active = true
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
            }
            };
        }
    }
}
