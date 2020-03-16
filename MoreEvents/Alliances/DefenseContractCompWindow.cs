using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Alliances
{
    public enum FightersLevel : int
    {
        RegularFighers = 350,
        EliteFighters = 1200,
        Officiers = 2000
    };

    public static class FightersLevelUtils
    {
        public static string ToStringHuman(this FightersLevel fightersLevel)
        {
            switch (fightersLevel)
            {
                case FightersLevel.RegularFighers:
                    return "FightersLevel_RegularFighers".Translate();
                case FightersLevel.EliteFighters:
                    return "FightersLevel_EliteFighters".Translate();
                case FightersLevel.Officiers:
                    return "FightersLevel_Officiers".Translate();
                default:
                    return "";
            }
        }
    }

    public class DefenseContractCompWindow : Window
    {
        private Alliance alliance;
        private DefenseContractCompProperties defenseContractCompProperties;
        private Pawn negotiator;

        public override Vector2 InitialSize => new Vector2(700, 700);

        private int contractDaysDuration;
        private string contractDaysDurationBuff;

        private int maxContractDays => 30;
        private int minContractDays => 5;

        private int totalDaysCost => contractDaysDuration * trustCostPerDay;
        private int totalCost => totalDaysCost + fightersCost[fightersType];

        private List<Pawn> generatedPawns = new List<Pawn>(3);

        private FactionInteraction faction;
        private FightersLevel fightersType;

        private int trustCostPerDay => defenseContractCompProperties.TrustCostPerDay;
        private Dictionary<FightersLevel, int> fightersCost => defenseContractCompProperties.FightersCost;

        private FactionInteraction playerInteraction => QuestsManager.Communications.FactionManager.PlayerInteraction;

        public DefenseContractCompWindow()
        {

        }

        public DefenseContractCompWindow(Alliance alliance, DefenseContractCompProperties defenseContractCompProperties, Pawn negotiator)
        {
            this.alliance = alliance;
            this.defenseContractCompProperties = defenseContractCompProperties;
            this.negotiator = negotiator;

            fightersType = FightersLevel.RegularFighers;
            faction = alliance.Factions.FirstOrDefault();

            doCloseX = true;
            forcePause = true;

            contractDaysDuration = minContractDays;
        }

        private void ResetBufferTo(int days, ref int val, ref string buff)
        {
            val = days;
            buff = days.ToString();
        }

        public override void DoWindowContents(Rect inRect)
        {
            float y = inRect.y + 10;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect selectFactionRect = new Rect(0, y, inRect.width, 25);
            if(GUIUtils.DrawCustomButton(selectFactionRect, "DefenseContractCompWindow_SelectFaction".Translate(faction.Faction.Name), Color.white))
            {
                List<FloatMenuOption> opt = new List<FloatMenuOption>();
                foreach (var f in alliance.Factions)
                {
                    opt.Add(new FloatMenuOption($"{f.Faction.Name} {((int)f.Faction.def.techLevel < (int)defenseContractCompProperties.MinFactionTechLevel ? "DefenseContractCompWindow_LowTechLevel".Translate().ToString() : "")}", () => { if ((int)f.Faction.def.techLevel > (int)defenseContractCompProperties.MinFactionTechLevel) faction = f; }));
                }
                Find.WindowStack.Add(new FloatMenu(opt));
            }
            y += 30;
            Rect durationLabelRect = new Rect(0, y, inRect.width, 30);
            Widgets.Label(durationLabelRect, "DefenseContractCompWindow_ContractDuration".Translate());
            durationLabelRect.y += 30;
            Widgets.IntEntry(durationLabelRect, ref contractDaysDuration, ref contractDaysDurationBuff);
            if (contractDaysDuration < minContractDays)
                ResetBufferTo(minContractDays, ref contractDaysDuration, ref contractDaysDurationBuff);
            else if (contractDaysDuration > maxContractDays)
                ResetBufferTo(maxContractDays, ref contractDaysDuration, ref contractDaysDurationBuff);

            y = 100;
            Rect costRangeRect = new Rect(0, y, inRect.width, 25);
            Widgets.Label(costRangeRect, "DefenseContractCompWindow_CostRangeSlider".Translate());
            costRangeRect.y += 28;
            if(GUIUtils.DrawCustomButton(costRangeRect, fightersType.ToStringHuman(), Color.white))
            {
                List<FloatMenuOption> opt = new List<FloatMenuOption>();
                foreach(FightersLevel f in Enum.GetValues(typeof(FightersLevel)))
                {
                    opt.Add(new FloatMenuOption(f.ToStringHuman(), () => fightersType = f));
                }
                Find.WindowStack.Add(new FloatMenu(opt));
            }
            y += 60;
            Rect generateButtonRect = new Rect(0, y, inRect.width, 25);
            if (GUIUtils.DrawCustomButton(generateButtonRect, "DefenseContractCompWindow_ShowExamplePawns".Translate(), Color.white))
            {
                GenerateAndSelectPawns();
            }
            y += 30;
            Widgets.DrawLineHorizontal(0, y, inRect.width);
            y += 35;
            Rect pawnRect = new Rect(0, y, 100, 140);
            float weaponY = pawnRect.y + 135;
            Rect weaponRect = new Rect(0, weaponY, 100, 100);
            foreach(var pawn in generatedPawns)
            {
                Widgets.ThingIcon(pawnRect, pawn);
                if(pawn.equipment != null && pawn.equipment.Primary != null)
                {
                    Widgets.ThingIcon(weaponRect, pawn.equipment.Primary);
                }

                weaponRect.x += 110;
                pawnRect.x += 110;
            }

            Text.Anchor = TextAnchor.UpperLeft;

            y += 280;

            Rect infoRect = new Rect(0, y, inRect.width, 200);
            Widgets.Label(infoRect, "DefenseContractCompWindow_ResultInfo".Translate(contractDaysDuration, totalDaysCost, fightersType.ToStringHuman(), fightersCost[fightersType], totalCost));

            Text.Anchor = TextAnchor.MiddleCenter;
            y += 210;
            Rect createAgrRect = new Rect(0, inRect.height - 30, inRect.width, 25);
            bool active = faction.Trust >= totalCost;
            if (GUIUtils.DrawCustomButton(createAgrRect, "DefenseContractCompWindow_CreateAgreement".Translate(), active ? Color.white : Color.gray))
            {
                if(active)
                {
                    CreateAgreement();

                    Close();
                }
                else
                {
                    Messages.Message("DefenseContractCompWindow_CreateAgreement_NoTrust".Translate(), MessageTypeDefOf.NegativeEvent, false);
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void CreateAgreement()
        {
            faction.Trust -= totalCost;

            DefenseContractComp defenseContractComp = new DefenseContractComp(alliance, faction, playerInteraction, fightersType, contractDaysDuration);
            alliance.AddAgreement(defenseContractComp);

            Find.LetterStack.ReceiveLetter("DefenseContractCompWindow_CreatedTitle".Translate(), "DefenseContractCompWindow_CreatedDesc".Translate(contractDaysDuration, faction.Faction.Name), LetterDefOf.PositiveEvent);
        }

        private void GenerateAndSelectPawns()
        {
            generatedPawns.Clear();

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = faction.Faction,
                groupKind = PawnGroupKindDefOf.Combat,
                points = (int)fightersType
            };

            foreach(var p in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms))
            {
                if (generatedPawns.Count == 6)
                    break;

                generatedPawns.Add(p);
            }
        }
    }
}
