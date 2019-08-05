using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_KineticBuffer : MKArmorModule
    {
        public int KineticBuffer = 0;
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref KineticBuffer, "buffer");
        }

        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_KineticBuffer".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            if (!Armor.Active)
                return;

            if (KineticBuffer >= 100)
                return;

            KineticBuffer = (int)Mathf.Clamp(KineticBuffer + (dInfo.Amount * 0.2f), 0, 100);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Armor.Active)
            {
                yield return new Command_Action()
                {
                    icon = def.Item.uiIcon,
                    defaultLabel = "ArmorModuleWorker_KineticBuffer_Action".Translate(KineticBuffer),
                    defaultDesc = "ArmorModuleWorker_KineticBuffer_Action_Desc".Translate(),
                    action = delegate
                    {
                        if (KineticBuffer < 100)
                        {
                            Messages.Message("ArmorModuleWorker_KineticBuffer_NotEnoughBuffer".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        KineticBuffer = 0;

                        Utils.DoExplosionPlus(Armor.Wearer.Position, (IntVec3 x) => x == Armor.Wearer.Position, Armor.Wearer.Map, 4.0f, DamageDefOf.Stun, Armor.Wearer);
                    }
                };
            }
        }
    }
}
