using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RandomPlaces
{
    public class ExtraLord_AttackPlayer : ExtraLord
    {
        public override Lord GetLord(Faction faction, Map map)
        {
            LordJob_AssaultColony lordJob_AssaultColony = new LordJob_AssaultColony(faction, false, false, false, false, false);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob_AssaultColony, map);

            return lord;
        }
    }
}
