using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_Weather : EmailMessageWorker
    {
        public override bool PreReceived(EmailMessage message, EmailBox box)
        {
            base.PreReceived(message, box);

            var settlementBase = RandomNearbyTradeableSettlement(Find.AnyPlayerHomeMap.Tile, message.Faction);
            if (settlementBase != null)
            {
                WeatherDef weather = DefDatabase<WeatherDef>.GetRandom();
                message.Message = string.Format(message.Message, box.Owner.Name, settlementBase.Name, weather.LabelCap.ToLower());

                return true;
            }
            else
            {
                return false;
            }
        }

        private Settlement RandomNearbyTradeableSettlement(int originTile, Faction faction)
        {
            return Find.WorldObjects.Settlements.Where(delegate (Settlement settlement)
            {
                if (settlement.Faction != faction)
                    return false;

                return Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile) < 36f && Find.WorldReachability.CanReach(originTile, settlement.Tile);
            }).RandomElementWithFallback();
        }

    }
}
