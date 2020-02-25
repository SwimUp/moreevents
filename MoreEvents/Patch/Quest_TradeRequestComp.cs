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
    /*
    public class Quest_TradeRequestComp : QuestRim.Quest
    {
        public override string CardLabel => "Quest_TradeRequestComp_CardLabel".Translate();

        public override string Description => "LetterCaravanRequest".Translate(SettlementBase.Label, TradeRequestUtility.RequestedThingLabel(Component.requestThingDef, Component.requestCount).CapitalizeFirst(), (Component.requestThingDef.GetStatValueAbstract(StatDefOf.MarketValue) * (float)Component.requestCount).ToStringMoney("F0"), GenThing.ThingsToCommaList(Component.rewards, useAnd: true).CapitalizeFirst(), GenThing.GetMarketValue(Component.rewards).ToStringMoney("F0"), (Component.expiration - Find.TickManager.TicksGame).ToStringTicksToDays("F0"), CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Tile, SettlementBase.Tile, null).ToStringTicksToDays("0.#"));

        public int Tile;

        public WorldObject SettlementBase;

        public override int SuccessTrustAffect => 10;
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

        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

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

        public override void GameLoaded()
        {
            if(Component == null || !Component.ActiveRequest)
            {
                Log.Warning($"Somehow the component has been null or inactive request. Removing.");
                QuestsManager.Communications.RemoveQuest(this);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref SettlementBase, "SettlementBase");
            Scribe_Values.Look(ref Tile, "Tile");
        }
    }
    */
}
