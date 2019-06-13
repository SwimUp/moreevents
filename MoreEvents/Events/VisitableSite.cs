using MoreEvents.Biomes;
using MoreEvents.Events.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events
{
    public class VisitableSite : MapParent
    {
        public bool RemoveAfterLeave = true;

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(caravan))
            {
                yield return floatMenuOption;
            }
            if (!HasMap)
            {
                foreach (FloatMenuOption floatMenuOption2 in GetFloatMenuOptions(caravan, this))
                {
                    yield return floatMenuOption2;
                }
            }
        }

        public virtual IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return null;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (base.HasMap)
            {
                yield return LeaveCommand(base.Map);
            }
        }

        public virtual bool CanLeave()
        {
            return true;
        }

        private Command LeaveCommand(Verse.Map map)
        {
            Command_Action command = new Command_Action();
            command.defaultLabel = Translator.Translate("ShipSite_LeaveCommandLabel");
            command.defaultDesc = Translator.Translate("ShipSite_LeaveCommandDesc");
            command.icon = ContentFinder<Texture2D>.Get("Map/leaving-queue");
            command.action = delegate
            {
                if (CanLeave())
                {
                    PreForceReform(this);
                }
            };

            if (map.mapPawns.FreeColonistsCount == 0)
            {
                command.Disable();
            }

            return command;
        }

        public virtual void PreForceReform(MapParent mapParent)
        {
            ForceReform(mapParent);
        }

        public void ForceReform(MapParent mapParent)
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

            if(RemoveAfterLeave)
                Find.WorldObjects.Remove(mapParent);
        }
    }
}
