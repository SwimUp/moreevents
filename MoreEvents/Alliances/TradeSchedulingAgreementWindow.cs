using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Alliances
{
    public class TradeSchedulingAgreementWindow : Window
    {
        private Alliance alliance;
        private FactionInteraction factionInteraction;

        private Settlement settlement;

        private string DaysLabelHolder => "TradeSchedulingAgreementWindow_DaysLabelHolder".Translate();

        public override Vector2 InitialSize => new Vector2(1000, 700);

        private TradeSchedulingAgreementCompProperties tradeSchedulingAgreementCompProperties;

        private Dictionary<Settlement, float> factionSettlements = new Dictionary<Settlement, float>();

        public TradeSchedulingAgreementWindow()
        {
            doCloseX = true;
            forcePause = true;
        }

        public TradeSchedulingAgreementWindow(Alliance alliance, TradeSchedulingAgreementCompProperties tradeSchedulingAgreementComp)
        {
            doCloseX = true;
            forcePause = true;

            this.alliance = alliance;
            this.tradeSchedulingAgreementCompProperties = tradeSchedulingAgreementComp;

            SelectNewFaction(alliance.Factions.First());
        }

        public void SelectNewFaction(FactionInteraction faction)
        {
            factionInteraction = faction;

            factionSettlements = Find.WorldObjects.Settlements.Where(x => x.Faction == factionInteraction.Faction).ToDictionary(k => k, v => CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Find.AnyPlayerHomeMap.Tile, v.Tile, null).TicksToDays());

            settlement = factionSettlements.First().Key;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float y = inRect.y + 10;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(inRect.x, y, inRect.width, 30), "TradeSchedulingAgreementWindow_SelectFactionLabel".Translate());
            y += 31;
            Text.Font = GameFont.Medium;
            if(GUIUtils.DrawCustomButton(new Rect(inRect.x, y, inRect.width, 30), factionInteraction?.Faction.Name, Color.white))
            {
                List<FloatMenuOption> factions = new List<FloatMenuOption>();
                foreach (var faction in alliance.Factions)
                    factions.Add(new FloatMenuOption(faction.Faction.Name, delegate {
                        SelectNewFaction(faction);
                    }));

                Find.WindowStack.Add(new FloatMenu(factions));
            }

            y += 40;
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(inRect.x, y, inRect.width, 30), "TradeSchedulingAgreementWindow_SelectSettlementLabel".Translate());
            y += 31;
            Text.Font = GameFont.Medium;
            string settlementInfo = settlement == null ? string.Empty : "TradeSchedulingAgreementWindow_SelectedSettlement".Translate(settlement.Name, factionSettlements[settlement].ToString("f2"));
            if (GUIUtils.DrawCustomButton(new Rect(inRect.x, y, inRect.width, 30), settlementInfo, Color.white))
            {
                List<FloatMenuOption> settlements = new List<FloatMenuOption>();

                foreach(var settl in factionSettlements)
                {
                    settlements.Add(new FloatMenuOption($"{settl.Key.Name} - {settl.Value.ToString("f2")} {DaysLabelHolder}", delegate
                    {
                        settlement = settl.Key;
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(settlements));
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft; ;
        }
    }
}
