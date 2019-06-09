using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things
{
    public class Item_ColdFusionRectorHeart : ThingWithComps
    {
        //GenConstruct

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
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
            command.icon = ContentFinder<Texture2D>.Get("Things/Install");
            command.action = delegate
            {
                Designator_Build des = new Designator_Build(ThingDefOfLocal.ColdFusionReactor);
                Find.DesignatorManager.Select(des);
            };

            return command;
        }
    }
}
