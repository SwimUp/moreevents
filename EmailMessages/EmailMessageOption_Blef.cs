using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents;
using QuestRim;
using RimWorld;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_Blef : EmailMessageOption
    {
        public override string Label => "Blef".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            EmailMessage msg = box.FormMessageFrom(message.Faction, "Blef_Explain".Translate(), "Blef_Subject".Translate());

            box.SendMessage(msg);

            Map map = Find.AnyPlayerHomeMap;
            int count = Rand.Range(2, 4);

            for (int i = 0; i < count; i++)
            {
                if (FindHomeAreaCell(map, out IntVec3 result))
                {
                    ThingDef bomb = ThingDefOfLocal.TrapIED_HighExplosive;
                    Thing thing = GenSpawn.Spawn(bomb, result, map);
                    thing.SetFaction(message.Faction);
                }
            }
        }

        private bool FindHomeAreaCell(Map map, out IntVec3 result)
        {
            List<IntVec3> cells = (from c in map.areaManager.Home.ActiveCells where c.GetRoof(map) != RoofDefOf.RoofRockThick && !c.Fogged(map) && c.GetFirstBuilding(map) == null select c).ToList();
            if (cells.Count > 0)
            {
                result = cells.RandomElement();
                return true;
            }

            result = IntVec3.Invalid;
            return false;
        }
    }
}
