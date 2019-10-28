using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.ShipCrash
{
    public class ShipSite : MapParent
    {
        public ShipMapGenerator Generator
        {
            get
            {
                if(generator == null)
                {
                    generator = (ShipMapGenerator)Activator.CreateInstance(Type.GetType(GeneratorType));
                }

                return generator;
            }
        }
        private ShipMapGenerator generator;

        public string GeneratorType;

        public override string Label => Generator.ExpandLabel;

        public override Texture2D ExpandingIcon => Generator.ExpandTexture;

        public override bool AppendFactionToInspectString => false;

        public override Material Material
        {
            get
            {
                if (cachedMat == null)
                {
                    cachedMat = MaterialPool.MatFrom(ExpandingIcon);
                }

                return cachedMat;
            }
        }
        private Material cachedMat = null;

        public void SetGenerator(ShipMapGenerator generator)
        {
            this.generator = generator;
            GeneratorType = $"MoreEvents.Events.ShipCrash.Map.MapGenerator.{generator.GetType().Name}";
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(caravan))
            {
                yield return floatMenuOption;
            }

            foreach (FloatMenuOption floatMenuOption2 in GetFloatMenuOptions(caravan, this))
            {
                yield return floatMenuOption2;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref GeneratorType, "GeneratorType");
        }

        private IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CaravanArrivalAction_EnterToShipCrash.CanVisit(caravan, mapParent), () => new CaravanArrivalAction_EnterToShipCrash(mapParent, Generator), "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Generator.Description);
            stringBuilder.Append(base.GetInspectString());

            return stringBuilder.ToString();
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

        private Command LeaveCommand(Verse.Map map)
        {
            Command_Action command = new Command_Action();
            command.defaultLabel = Translator.Translate("ShipSite_LeaveCommandLabel");
            command.defaultDesc = Translator.Translate("ShipSite_LeaveCommandDesc");
            command.icon = ContentFinder<Texture2D>.Get("Map/leaving-queue");
            command.action = delegate
            {
                ForceReform(this);
            };

            if(map.mapPawns.FreeColonistsCount == 0)
            {
                command.Disable();
            }

            return command;
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }
        public static void ForceReform(MapParent mapParent)
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(mapParent.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return;
            }

            if (Dialog_FormCaravan.AllSendablePawns(mapParent.Map, reform: true).Any((Pawn x) => x.IsColonist))
            {
                Messages.Message("MessageYouHaveToReformCaravanNow".Translate(), new GlobalTargetInfo(mapParent.Tile), MessageTypeDefOf.NeutralEvent);
                Current.Game.CurrentMap = mapParent.Map;
                Dialog_FormCaravan window = new Dialog_FormCaravan(mapParent.Map, reform: true, delegate
                {
                    if (mapParent.HasMap)
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
            Find.WorldObjects.Remove(mapParent);
        }
    }
}
