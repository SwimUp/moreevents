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

                    if (armor != null)
                    {
                        foreach(var slot in armor.StatsListeners)
                        {
                            if(slot.def.StatAffecter.ContainsKey(parentStat))
                            {
                                return slot.StatDescription();
                            }
                        }
                    }
                }
                else if (req.Thing is Apparel_MkArmor armor)
                {
                    foreach (var slot in armor.StatsListeners)
                    {
                        if (slot.def.StatAffecter.ContainsKey(parentStat))
                        {
                            return slot.StatDescription();
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

                    if (armor != null)
                    {
                        foreach (var slot in armor.StatsListeners)
                        {
                            if (slot.def.StatAffecter.ContainsKey(parentStat))
                            {
                                slot.TransformStat(parentStat, ref val);
                            }
                        }
                    }
                }else if(req.Thing is Apparel_MkArmor armor)
                {
                    foreach (var slot in armor.StatsListeners)
                    {
                        if (slot.def.StatAffecter.ContainsKey(parentStat))
                        {
                            slot.TransformStat(parentStat, ref val);
                        }
                    }
                }
            }
        }
    }
}
