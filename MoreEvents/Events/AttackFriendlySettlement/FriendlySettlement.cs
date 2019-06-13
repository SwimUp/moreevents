using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class FriendlySettlement : VisitableSite
    {
        public CaravanArrivalAction_HelpFriendlySettlement caravanAction;

        public bool AttackRepelled = false;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RemoveAfterLeave = true;

            caravanAction = new CaravanArrivalAction_HelpFriendlySettlement(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AttackRepelled, "AttackRepelled");
        }

        public override bool CanLeave()
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(this.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if(!AttackRepelled)
            {
                Messages.Message(Translator.Translate("AttackIsNotRepelled"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }
    }
}
