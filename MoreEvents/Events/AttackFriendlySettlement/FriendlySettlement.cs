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

        public override void ForceReform(MapParent mapParent)
        {
            if (Dialog_FormCaravan.AllSendablePawns(mapParent.Map, reform: true).Any((Pawn x) => x.IsColonist))
            {
                Messages.Message("MessageYouHaveToReformCaravanNow".Translate(), new GlobalTargetInfo(mapParent.Tile), MessageTypeDefOf.NeutralEvent);
                Current.Game.CurrentMap = mapParent.Map;
                Dialog_FormCaravan window = new Dialog_FormCaravan(mapParent.Map, reform: true, delegate
                {
                    if (RemoveAfterLeave && mapParent.HasMap)
                    {
                        Find.WorldObjects.Remove(mapParent);
                    }

                }, mapAboutToBeRemoved: true);
                Find.WindowStack.Add(window);
                for (int i = window.transferables.Count - 1; i >= 0; i--)
                {
                    TransferableOneWay t = window.transferables[i];
                    if (t != null)
                    {
                        if (t.AnyThing != null)
                        {
                            if (t.AnyThing.Faction != null && t.AnyThing.Faction == Faction)
                            {
                                window.transferables.Remove(t);
                            }
                        }
                    }
                }
                return;
            }
            List<Pawn> tmpPawns = new List<Pawn>();
            tmpPawns.Clear();
            tmpPawns.AddRange(from x in mapParent.Map.mapPawns.AllPawns
                              where x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer
                              select x);
            if (tmpPawns.Any((Pawn x) => CaravanUtility.IsOwner(x, Faction.OfPlayer)))
            {
                CaravanExitMapUtility.ExitMapAndCreateCaravan(tmpPawns, Faction.OfPlayer, mapParent.Tile, mapParent.Tile, -1);
            }
            tmpPawns.Clear();

            if (RemoveAfterLeave)
                Find.WorldObjects.Remove(mapParent);
        }
    }
}
