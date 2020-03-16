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
        enum DeliveryMethod : byte
        {
            Caravan,
            Capsule
        }

        public class AgreementGood
        {
            public Thing Thing;

            public int CountToTransfer;

            public string EditBuffer;

            public int Value;

            public AgreementGood(Thing good, int value)
            {
                Thing = good;
                Value = value;
            }
        }

        private Vector2 goodsSlider;

        private Alliance alliance;
        private FactionInteraction factionInteraction;

        private Settlement settlement;

        private Pawn negotiator;

        private string DaysLabelHolder => "TradeSchedulingAgreementWindow_DaysLabelHolder".Translate();

        public override Vector2 InitialSize => new Vector2(1000, 700);

        private TradeSchedulingAgreementCompProperties tradeSchedulingAgreementCompProperties;

        private Dictionary<Settlement, float> factionSettlements = new Dictionary<Settlement, float>();

        private List<AgreementGood> settlementGoods = new List<AgreementGood>();

        private float multiplierPerItemMass => tradeSchedulingAgreementCompProperties.multiplierPerItemMass;
        private float prepareMultiplierPerItem => tradeSchedulingAgreementCompProperties.prepareMultiplierPerItem;

        private int totalCost;
        private float agreementDelay;
        private float formCaravanDelay;
        private int trustAdditional;
        private string trustEditBuffer;

        private int trustCost => tradeSchedulingAgreementCompProperties.trustCost;

        private int totalTrustCost => trustCost + trustAdditional;

        private float discount;
        private float baseDiscount => tradeSchedulingAgreementCompProperties.baseDiscount;

        private int maxAdditionalTrust => tradeSchedulingAgreementCompProperties.maxAdditionalTrust;

        private float maxDiscount => tradeSchedulingAgreementCompProperties.maxDiscount;

        private float discountPerAdditionalTrust => tradeSchedulingAgreementCompProperties.discountPerAdditionalTrust;

        private DeliveryMethod deliveryMethod;

        public TradeSchedulingAgreementWindow()
        {
            doCloseX = true;
            forcePause = true;

            discount = baseDiscount;
        }

        public TradeSchedulingAgreementWindow(Alliance alliance, TradeSchedulingAgreementCompProperties tradeSchedulingAgreementComp, Pawn negotiator)
        {
            this.tradeSchedulingAgreementCompProperties = tradeSchedulingAgreementComp;

            discount = baseDiscount;

            this.negotiator = negotiator;

            doCloseX = true;
            forcePause = true;

            this.alliance = alliance;

            SelectNewFaction(alliance.Factions.First());

            UpdateDeliveryMethod();
        }

        public void SelectNewFaction(FactionInteraction faction)
        {
            factionInteraction = faction;

            factionSettlements = Find.WorldObjects.Settlements.Where(x => x.Faction == factionInteraction.Faction)
                .ToDictionary(k => k, v => CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Find.AnyPlayerHomeMap.Tile, v.Tile, null).TicksToDays());

            SelectNewSettlement(factionSettlements.First().Key);

            UpdateDeliveryMethod();
        }

        public void SelectNewSettlement(Settlement settlement)
        {
            this.settlement = settlement;

            settlementGoods.Clear();
            foreach (var good in settlement.Goods)
            {
                if (good.def == ThingDefOf.Silver)
                    continue;

                int value = RecalculateItemValue(good);
                settlementGoods.Add(new AgreementGood(good, value));
            }
        }

        private int RecalculateItemValue(Thing thing)
        {
            int val = (int)(TradeUtility.GetPricePlayerBuy(thing, settlement.trader.TraderKind.PriceTypeFor(thing.def, TradeAction.PlayerBuys).PriceMultiplier(), negotiator.GetStatValue(StatDefOf.TradePriceImprovement), settlement.trader.TradePriceImprovementOffsetForPlayer) * discount);

            return Mathf.Clamp(val, 1, val);
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
            string settlementInfo = settlement == null ? string.Empty : "TradeSchedulingAgreementWindow_SelectedSettlement".Translate(settlement.Name, factionSettlements[settlement].ToString("f2")).ToString();
            if (GUIUtils.DrawCustomButton(new Rect(inRect.x, y, inRect.width, 30), settlementInfo, Color.white))
            {
                List<FloatMenuOption> settlements = new List<FloatMenuOption>();

                foreach(var settl in factionSettlements.Where(x => x.Value > 0f).OrderBy(x2 => x2.Value))
                {
                    settlements.Add(new FloatMenuOption($"{settl.Key.Name} - {settl.Value.ToString("f2")} {DaysLabelHolder}", delegate
                    {
                        SelectNewSettlement(settl.Key);
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(settlements));
            }
            y += 35;
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(inRect.x, y, inRect.width, 50), "TradeSchedulingAgreementWindow_SelectItems".Translate());
            y += 55;
            if (settlementGoods != null)
            {
                int tmpY = 0;
                Rect goodReect = new Rect(0, tmpY, inRect.width - 100, 27);
                Rect goodsMainRect = new Rect(inRect.x, y, inRect.width, 280);
                Rect scrollVertRectFact = new Rect(0, 0, inRect.width - 20, settlementGoods.Count * 35);
                Widgets.BeginScrollView(goodsMainRect, ref goodsSlider, scrollVertRectFact, true);
                for (int i = 0; i < settlementGoods.Count; i++)
                {
                    AgreementGood good = settlementGoods[i];

                    DrawGood(goodReect, ref tmpY, good);
                }
                Widgets.EndScrollView();
            }

            y += 290;

            CacheData();

            Rect trustButton = new Rect(inRect.x, y, 350, 27);
            int tmpMaxTrust = factionInteraction.Trust < maxAdditionalTrust ? factionInteraction.Trust : maxAdditionalTrust;
            Widgets.Label(trustButton, "TradeSchedulingAgreementWindow_TrustAdditionalLabel".Translate(tmpMaxTrust));
            trustButton.width = 200;
            trustButton.x += 355;
            Widgets.IntEntry(trustButton, ref trustAdditional, ref trustEditBuffer);
            trustButton.x += 210;
            ClampTrust(ref tmpMaxTrust);
            if (GUIUtils.DrawCustomButton(trustButton, "TradeSchedulingAgreementWindow_TrustAdditionaRecalculate".Translate(), Color.white))
            {
                RecalculateCosts();
            }

            y += 27;

            Text.Anchor = TextAnchor.UpperLeft;

            Rect resultLabelRect = new Rect(inRect.x, y, inRect.width, 105);
            Widgets.Label(resultLabelRect, "TradeSchedulingAgreementWindow_ResultInfo".Translate(totalCost,
                deliveryMethod == DeliveryMethod.Caravan ? "TradeSchedulingAgreementWindow_CaravanDelivery".Translate(factionSettlements[settlement].ToString("f2")) : "TradeSchedulingAgreementWindow_CapsuleDelivery".Translate(),
                agreementDelay.ToString("f2"), trustCost, trustAdditional));

            y += 110;

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect resultButton = new Rect(inRect.x, y, inRect.width, 27);
            bool canSign = CanSign(out string reason);
            if(GUIUtils.DrawCustomButton(resultButton, "TradeSchedulingAgreementWindow_CreateAgreement".Translate(), canSign ? Color.white : Color.gray))
            {
                if(!canSign)
                {
                    Messages.Message("TradeSchedulingAgreementWindow_CantSignRightNow".Translate(reason), MessageTypeDefOf.NegativeEvent);
                }
                else
                {
                    RecalculateCosts();

                    CreateAgreement(alliance, settlement, totalCost, (int)agreementDelay, settlementGoods.Where(x => x.CountToTransfer > 0).ToList(), factionInteraction, totalTrustCost, this, (int)formCaravanDelay);
                }
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public static void CreateAgreement(Alliance alliance, Settlement settlement, int totalCost, int agreementDelay, List<AgreementGood> items, FactionInteraction signer, int trust, Window windowToClose, int formCaravanDelay)
        {
            if (totalCost == 0)
                return;

            if (agreementDelay == 0)
                return;

            if (items == null)
                return;

            Map targetMap = Find.Maps.Where(x => x.IsPlayerHome).First(x2 => x2.resourceCounter.Silver >= totalCost);
            if (PriceUtils.TakeSilverFromPlayer(totalCost, targetMap))
            {
                List<Thing> toBuy = new List<Thing>();
                foreach (var item in items)
                {
                    if (item.Thing is Pawn pawn)
                    {
                        toBuy.Add(item.Thing);

                        settlement.trader.StockListForReading.Remove(item.Thing);

                        if (Find.WorldPawns.Contains(pawn))
                        {
                            Find.WorldPawns.RemovePawn(pawn);
                        }
                    }
                    else
                    {
                        toBuy.Add(item.Thing.SplitOff(item.CountToTransfer));

                        if (item.Thing.stackCount == 0)
                            settlement.trader.StockListForReading.Remove(item.Thing);

                    }
                }

                signer.Trust -= trust;

                TradeSchedulingAgreementComp comp = new TradeSchedulingAgreementComp(alliance, signer, toBuy, settlement, targetMap, agreementDelay * 60000, formCaravanDelay * 60000);
                alliance.AddAgreement(comp);

                Find.LetterStack.ReceiveLetter("TradeSchedulingAgreement_CreateSucessTitle".Translate(), "TradeSchedulingAgreement_CreateSucessDesc".Translate(settlement.Faction.Name, totalCost, settlement.Name, agreementDelay), LetterDefOf.PositiveEvent);

                if (windowToClose != null)
                    windowToClose.Close();
            }
        }

        private void UpdateDeliveryMethod()
        {
            deliveryMethod = (int)factionInteraction.Faction.def.techLevel >= 3 ? DeliveryMethod.Capsule : DeliveryMethod.Caravan;
        } 

        private bool CanSign(out string reason)
        {
            reason = string.Empty;
            if(totalCost <= 0)
            {
                reason = "TradeSchedulingAgreementWindow_ZeroCost".Translate();
                return false;
            }

            if (!Find.Maps.Where(x => x.IsPlayerHome).Any(x2 => x2.resourceCounter.Silver >= totalCost))
            {
                reason = "TradeSchedulingAgreementWindow_NotEnoughSilver".Translate(totalCost);
                return false;
            }

            if (factionInteraction.Trust < totalTrustCost)
            {
                reason = "TradeSchedulingAgreementWindow_NotEnoughTrust".Translate(factionInteraction.Trust, totalTrustCost);
                return false;
            }

            return true;
        }

        private void CacheData()
        {
            List<AgreementGood> toAgreement = settlementGoods.Where(good => good.CountToTransfer > 0).ToList();

            totalCost = toAgreement.Sum(item => item.Value * item.CountToTransfer);

            int countItems = 0;
            float totalMass = 0;
            foreach(var item in toAgreement)
            {
                countItems++;

                totalMass += item.CountToTransfer* item.Thing.def.BaseMass;
            }
            formCaravanDelay = totalMass * multiplierPerItemMass + (countItems * prepareMultiplierPerItem);
            agreementDelay = formCaravanDelay + (deliveryMethod == DeliveryMethod.Caravan ? factionSettlements[settlement] : 0);
        }

        private void ClampTrust(ref int maxTrust)
        {
            trustAdditional = Mathf.Clamp(trustAdditional, 0, maxTrust);
            trustEditBuffer = trustAdditional.ToString();
        }

        private void RecalculateCosts()
        {
            discount = Mathf.Clamp(baseDiscount - (trustAdditional * discountPerAdditionalTrust), maxDiscount, baseDiscount);

            if (settlementGoods != null)
            {
                foreach (var good in settlementGoods)
                {
                    good.Value = RecalculateItemValue(good.Thing);
                }
            }
        }

        private void DrawGood(Rect rect, ref int y, AgreementGood good)
        {
            Rect thingIconRect = new Rect(0, y, 27, 27);
            Widgets.ThingIcon(thingIconRect, good.Thing.GetInnerIfMinified().def);
            Widgets.InfoCardButton(40f, y, good.Thing);

            Rect labelRect = new Rect(60f, y, rect.width - 200, 27);
            Widgets.Label(labelRect, $"TradeSchedulingAgreementWindow_DrawGood_Good".Translate(good.Thing.LabelCap, good.Value));

            Rect entryRect = new Rect(rect.width - 200, y, 270, 27);
            Widgets.IntEntry(entryRect, ref good.CountToTransfer, ref good.EditBuffer);
            good.CountToTransfer = Mathf.Clamp(good.CountToTransfer, 0, good.Thing.stackCount);
            good.EditBuffer = good.CountToTransfer.ToStringCached();

            if (good.CountToTransfer > 0)
                Widgets.DrawHighlight(new Rect(0, y, rect.width, 27));

            y += 35;
        }
    }
}
