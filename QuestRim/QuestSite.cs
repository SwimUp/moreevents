using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class QuestSite : MapParent
    {
        public override string Label => quest.PlaceLabel;

        public bool RemoveAfterLeave = true;

        public bool RemoveIfAllDie = true;

        public override Texture2D ExpandingIcon => quest.ExpandingIcon ?? def.ExpandingIconTexture;

        public Quest Quest => quest;

        private Quest quest;

        public override Material Material
        {
            get
            {
                if (cachedMat == null)
                {
                    cachedMat = MaterialPool.MatFrom(def.texture, ShaderDatabase.WorldOverlayTransparentLit, WorldMaterials.WorldObjectRenderQueue);
                }

                return cachedMat;
            }
        }
        private Material cachedMat = null;

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach(var opt in base.GetFloatMenuOptions(caravan))
            {
                yield return opt;
            }

            foreach (var opt in quest.GetFloatMenuOptions(caravan, this))
            {
                yield return opt;
            }
        }

        private Command LeaveCommand(Verse.Map map)
        {
            Command_Action command = new Command_Action();
            command.defaultLabel = Translator.Translate("ShipSite_LeaveCommandLabel");
            command.defaultDesc = Translator.Translate("ShipSite_LeaveCommandDesc");
            command.icon = ContentFinder<Texture2D>.Get("Map/leaving-queue");
            command.action = delegate
            {
                if (Quest.CanLeaveFromSite(this))
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

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if(!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public void PreForceReform(QuestSite mapParent)
        {
            if (Quest.PreForceReform(mapParent))
            {
                ForceReform(mapParent);
            }
        }

        public override void PostRemove()
        {
            Quest.PostSiteRemove(this);

            base.PostRemove();
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

                    Quest.PostForceReform(this);

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

            if (RemoveAfterLeave)
                Find.WorldObjects.Remove(mapParent);

            Quest.PostForceReform(this);
        }

        public override void Notify_CaravanFormed(Caravan caravan)
        {
            base.Notify_CaravanFormed(caravan);

            Quest.Notify_CaravanFormed(this, caravan);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach(var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (Quest.UseLeaveCommand && base.HasMap)
            {
                yield return LeaveCommand(base.Map);
            }

            foreach (var gizmo in quest.GetGizmos(this))
            {
                yield return gizmo;
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            foreach (var gizmo in base.GetCaravanGizmos(caravan))
            {
                yield return gizmo;
            }

            foreach (var gizmo in quest.GetCaravanGizmos(caravan))
            {
                yield return gizmo;
            }
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            Quest.PostMapGenerate(Map);
        }

        public override IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
        {
            foreach (var opt in base.GetTransportPodsFloatMenuOptions(pods, representative))
            {
                yield return opt;
            }

            foreach (var opt in quest.GetTransportPodsFloatMenuOptions(pods, representative))
            {
                yield return opt;
            }
        }

        public override string GetInspectString()
        {
            string text = quest.GetInspectString();
            return text;
        }

        public override void Tick()
        {
            base.Tick();

            quest.SiteTick();
        }

        public override string GetDescription()
        {
            return quest.GetDescription();
        }

        public void Init(Quest quest)
        {
            this.quest = quest;
        }

        public void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            quest.EndQuest(caravan, condition);

            if(Find.WorldObjects.Contains(this))
                Find.WorldObjects.Remove(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref quest, "Quest");
            Scribe_Values.Look(ref RemoveAfterLeave, "RemoveAfterLeave");
            Scribe_Values.Look(ref RemoveIfAllDie, "RemoveIfAllDie");
        }
    }
}
