using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_AblationCoating : MKArmorModule
    {
        private int shotsRemainin = 10;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref shotsRemainin, "shotsRemainin");
        }

        public override string StatDescription()
        {
            return "ArmorModuleWorker_AblationCoating".Translate();
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            if(shotsRemainin > 0)
            {
                absorb = true;
                shotsRemainin--;
            }

            if(shotsRemainin <= 0)
            {
                Armor.RemoveModule(def);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action()
            {
                icon = def.Item.uiIcon,
                defaultLabel = "ArmorModuleWorker_AblationCoating_Action".Translate(shotsRemainin),
                defaultDesc = "ArmorModuleWorker_AblationCoating_Action_Desc".Translate(),
                action = delegate
                {

                }
            };
        }
    }
}
