using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_SendCaravan : EmailMessageWorker
    {
        public int Days;
        public FloatRange TotalValue;

        public override void OnReceived(EmailMessage message, EmailBox box)
        {
            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms2 = default;
            parms2.totalMarketValueRange = TotalValue;
            parms2.filter = GetFilter();
            parms2.techLevel = message.Faction.def.techLevel;
            maker.fixedParams = parms2;

            AssistCaravanWithFixedInventoryComp assistComp = new AssistCaravanWithFixedInventoryComp(maker.Generate(), Days * 60000, message.Faction, Find.AnyPlayerHomeMap);
            assistComp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();
            QuestsManager.Communications.RegisterComponent(assistComp);
        }
        private ThingFilter GetFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);
            filter.SetAllow(ThingCategoryDefOf.Leathers, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);

            return filter;
        }

    }
}
