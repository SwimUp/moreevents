using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_Plato : MKArmorModule
    {
        public override float TransformValue => 10;

        public override string StatDescription()
        {
            return $"ArmorModuleWorker_Plato".Translate(TransformValue);
        }

        public override void TransformStat(StatDef def, ref float value)
        {
            value += 10;
        }
    }
}
