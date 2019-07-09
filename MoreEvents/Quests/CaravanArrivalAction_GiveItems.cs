using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Quests
{
    public class CaravanArrivalAction_GiveItems : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Quest_ThingsHelp Quest => quest;
        private Quest_ThingsHelp quest;

        public override string Label => "GiveItemsOption".Translate();

        public override string ReportString => "GiveItemsSuccessTitle".Translate();

        public CaravanArrivalAction_GiveItems()
        {
        }

        public CaravanArrivalAction_GiveItems(MapParent mapParent, Quest_ThingsHelp quest)
        {
            this.mapParent = mapParent;
            this.quest = quest;
        }

        public override void Arrived(Caravan caravan)
        {
            List<Thing> list = CaravanInventoryUtility.TakeThings(caravan, (Thing thing) =>
            {
                if (quest.RequestItems.ContainsKey(thing.def))
                {
                    int remaining = quest.RequestItems[thing.def];

                    int num = Mathf.Min(remaining, thing.stackCount);
                    remaining -= num;
                    quest.RequestItems[thing.def] = remaining;

                    return num;
                }

                return 0;
            });

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Destroy();
            }

            QuestSite site = (QuestSite)mapParent;
            site.EndQuest(caravan, EndCondition.Success);
        }

        public virtual FloatMenuAcceptanceReport CanGiveItems(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }

            foreach(var item in quest.RequestItems)
            {
                if(!CheckItems(caravan, item.Key, item.Value))
                {
                    return FloatMenuAcceptanceReport.WithFailReason("NotEnoughItemsToQuest".Translate());
                }
            }

            return true;
        }

        private bool CheckItems(Caravan caravan, ThingDef thingDef, int count)
        {
            return CaravanInventoryUtility.HasThings(caravan, thingDef, count);
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return FloatMenuAcceptanceReport.WithFailReason("NotEnoughItemsToQuest".Translate());
            }

            return CanGiveItems(caravan, mapParent);

        }
    }
}
