using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.ArmorStats
{
    public class ArmorStat : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing)
            {
                Pawn p = req.Thing as Pawn;
                if (p != null)
                {
                    var armor = Apparel_MkArmor.HasAnyMK(p);

                    if (armor != null) //&& armor.Active
                    {
                        foreach (var armorSlot in armor.Slots)
                        {
                            foreach (var slot in armorSlot.Modules)
                            {
                                if (slot.Module != null && slot.Module.def.StatAffecter.Contains(parentStat))
                                {
                                    return $"{armor.LabelCap}: x{slot.Module.TransformValue}";
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                Pawn p = req.Thing as Pawn;
                if (p != null)
                {
                    var armor = Apparel_MkArmor.HasAnyMK(p);

                    if (armor != null) //&& armor.Active
                    {
                        foreach (var armorSlot in armor.Slots)
                        {
                            foreach(var slot in armorSlot.Modules)
                            {
                                if(slot.Module != null && slot.Module.def.StatAffecter.Contains(parentStat))
                                {
                                    slot.Module.TransformStat(parentStat, ref val);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
