using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public static class WarUtility
    {
        public static Dictionary<TechLevel, FloatRange> ThreatRange = new Dictionary<TechLevel, FloatRange>
        {
            { TechLevel.Undefined, new FloatRange(250, 650) },
            { TechLevel.Animal, new FloatRange(250, 650) },
            { TechLevel.Neolithic, new FloatRange(250, 800) },
            { TechLevel.Medieval, new FloatRange(350, 1000) },
            { TechLevel.Industrial, new FloatRange(400, 1250) },
            { TechLevel.Spacer, new FloatRange(650, 1500) },
            { TechLevel.Ultra, new FloatRange(700, 2000) },
            { TechLevel.Archotech, new FloatRange(1000, 3500) }
        };

        public static War FirstWarWithPlayer(this FactionInteraction interaction)
        {
            FactionInteraction player = QuestsManager.Communications.FactionManager.PlayerInteraction;
            var alliance = QuestsManager.Communications.FactionManager.PlayerAlliance;
            var secondAlliance = interaction.Alliance;

            if (alliance == secondAlliance)
                return null;

            return interaction.InWars.FirstOrDefault(x => (x.DeclaredWarFaction == player && !x.AssaultFactions.Contains(interaction)) || (x.DefendingFaction == player && !x.DefendingFactions.Contains(interaction)) ||
            (x.AttackedAlliance != null && x.AttackedAlliance == alliance) || (x.DefendAlliance != null && x.DefendAlliance == alliance));
        }

        public static War FirstWarWith(this FactionInteraction interaction, FactionInteraction other)
        {
            var otherAlliance = other.Alliance;
            var firstAlliance = interaction.Alliance;

            if (firstAlliance == otherAlliance)
                return null;

            return interaction.InWars.FirstOrDefault(x => (x.DeclaredWarFaction == other && !x.AssaultFactions.Contains(interaction)) || (x.DefendingFaction == other && !x.DefendingFactions.Contains(interaction)) ||
            (x.AttackedAlliance != null && x.AttackedAlliance == otherAlliance) || (x.DefendAlliance != null && x.DefendAlliance == otherAlliance));
        }

        public static bool WarWithPlayer(War war)
        {
            FactionInteraction player = QuestsManager.Communications.FactionManager.PlayerInteraction;
            var alliance = QuestsManager.Communications.FactionManager.PlayerAlliance;

            return war.DeclaredWarFaction == player || war.DefendingFaction == player || war.AttackedAlliance == alliance || war.DefendAlliance == alliance;
        }

        public static string Translate(this Winner winner)
        {
            switch(winner)
            {
                case Winner.Assaulters:
                    return "Winner_Assaulters".Translate();
                case Winner.Defenders:
                    return "Winner_Defenders".Translate();
                case Winner.Draw:
                    return "Winner_Draw".Translate();
                case Winner.None:
                    return "Winner_None".Translate();
                default:
                    return "";
            }
        }

        public static float GetMultiplierFor(TechLevel techLevel)
        {
            float multiplier = 1f;

            switch (techLevel)
            {
                case TechLevel.Animal:
                    {
                        multiplier += Rand.Range(0.05f, 0.12f);
                        break;
                    }
                case TechLevel.Neolithic:
                    {
                        multiplier += Rand.Range(0.08f, 0.14f);
                        break;
                    }
                case TechLevel.Medieval:
                    {
                        multiplier += Rand.Range(0.09f, 0.16f);
                        break;
                    }
                case TechLevel.Industrial:
                    {
                        multiplier += Rand.Range(0.11f, 0.17f);
                        break;
                    }
                case TechLevel.Spacer:
                    {
                        multiplier += Rand.Range(0.12f, 0.19f);
                        break;
                    }
                case TechLevel.Ultra:
                    {
                        multiplier += Rand.Range(0.13f, 0.22f);
                        break;
                    }
                default:
                    {
                        multiplier += Rand.Range(0.01f, 0.22f);
                        break;
                    }
            }

            return multiplier;
        }

        public static float GetMultiplierFor(Hilliness hilliness)
        {
            switch(hilliness)
            {
                case Hilliness.SmallHills:
                        return 1.04f;
                case Hilliness.Mountainous:
                        return 1.06f;
                case Hilliness.LargeHills:
                        return 1.1f;
                default:
                        return 1f;
            }
        }

        public static FloatRange ThreatRangeFor(this TechLevel level)
        {
            return ThreatRange[level];
        }
    }
}