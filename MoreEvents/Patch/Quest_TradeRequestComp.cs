using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    public class Quest_TradeRequestComp : Quest
    {
        public override string CardLabel => "Quest_TradeRequestComp_CardLabel".Translate();

        public override string Description => "LetterCaravanRequest".Translate(SettlementBase.Label, TradeRequestUtility.RequestedThingLabel(component.requestThingDef, component.requestCount).CapitalizeFirst(), (component.requestThingDef.GetStatValueAbstract(StatDefOf.MarketValue) * (float)component.requestCount).ToStringMoney("F0"), GenThing.ThingsToCommaList(component.rewards, useAnd: true).CapitalizeFirst(), GenThing.GetMarketValue(component.rewards).ToStringMoney("F0"), (component.expiration - Find.TickManager.TicksGame).ToStringTicksToDays("F0"), CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Tile, SettlementBase.Tile, null).ToStringTicksToDays("0.#"));

        public int Tile;

        public WorldObject SettlementBase;
        public TradeRequestComp Component
        {
            get
            {
                if(component == null)
                {
                    component = SettlementBase.GetComponent<TradeRequestComp>();
                }

                return component;
            }
        }
        private TradeRequestComp component = null;
        public Quest_TradeRequestComp()
        {

        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public Quest_TradeRequestComp(TradeRequestComp comp, int tile)
        {
            component = comp;

            Tile = tile;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref SettlementBase, "SettlementBase");
            Scribe_Values.Look(ref Tile, "Tile");
        }
    }
}
