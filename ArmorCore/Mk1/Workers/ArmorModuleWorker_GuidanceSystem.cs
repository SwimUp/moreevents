using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_GuidanceSystem : MKArmorModule
    {
        public override float TransformValue => 15;

        public override string StatDescription()
        {
            return $"ArmorModuleWorker_GuidanceSystem".Translate(TransformValue);
        }

        public override void TransformStat(StatDef def, ref float value)
        {
            value += 15;
        }
    }
}
