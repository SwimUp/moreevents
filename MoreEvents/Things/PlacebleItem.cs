using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things
{
    public class PlacebleItem : ThingComp
    {
        public CompProperties_PlacebleItem Props => (CompProperties_PlacebleItem)props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            yield return CreateBlueprint();
        }

        private Command CreateBlueprint()
        {
            Command_Action command = new Command_Action();
            command.defaultLabel = Translator.Translate("InstallCommand_Title");
            command.defaultDesc = Translator.Translate("InstallCommand_Desc");
            command.icon = Props.PlaceDef.uiIcon;
            command.action = delegate
            {
                Designator_BuildWithoutDef des = new Designator_BuildWithoutDef(Props.PlaceDef);
                Find.DesignatorManager.Select(des);
            };

            return command;
        }
    }
}
