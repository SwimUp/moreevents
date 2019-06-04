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
        public ShipMapGenerator Generator { get; private set; }

        public override string Label => Generator.ExpandLabel;

        public override Texture2D ExpandingIcon => Generator.ExpandTexture;

        public bool OnMap = false;


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
            Generator = generator;
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

        private IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CaravanArrivalAction_EnterToShipCrash.CanVisit(caravan, mapParent), () => new CaravanArrivalAction_EnterToShipCrash(mapParent, Generator), "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Generator.Description);

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
                Log.Message("OK");
            };

            if(map.mapPawns.FreeColonistsCount == 0)
            {
                command.Disable();
            }

            return command;
        }
    }
}
