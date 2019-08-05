using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class MKArmorModule : MKModule
    {
        public ArmorModuleDef def;

        public Apparel_MkArmor Armor;

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_References.Look(ref Armor, "Armor");
        }

        public virtual bool CanAffectCondition(GameConditionDef cond)
        {
            return true;
        }

        public virtual void SetupStats(Apparel_MkArmor armor)
        {

        }

        public virtual void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {

        }


        public override string StatDescription()
        {
            return "";
        }

        public virtual void TransformStat(StatDef statDef, ref float value)
        {
            if (def.StatAffecter.ContainsKey(statDef))
            {
                value += def.StatAffecter[statDef];
            }
        }

        public virtual void DrawWornExtras()
        {

        }

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }
    }
}
