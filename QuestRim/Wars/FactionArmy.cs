using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim.Wars
{
    public class FactionArmy : IExposable
    {
        public FactionInteraction Faction;

        public bool UseForGenerator;

        public float DepletionFromWar
        {
            get
            {
                return depletionFromWar;
            }
            set
            {
                depletionFromWar = Mathf.Clamp(value, 0, 1);
            }
        }
        private float depletionFromWar;

        public Dictionary<ArmyOrderDef, int> lastGiveOrderTicks;

        public War War;

        public FactionArmy()
        {

        }

        public FactionArmy(FactionInteraction factionInteraction, War war, bool useForGenerator)
        {
            Faction = factionInteraction;
            UseForGenerator = useForGenerator;
            War = war;

            lastGiveOrderTicks = new Dictionary<ArmyOrderDef, int>();
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref War, "War");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Values.Look(ref depletionFromWar, "depletionFromWar");
            Scribe_Values.Look(ref UseForGenerator, "UseForGenerator");
            Scribe_Collections.Look(ref lastGiveOrderTicks, "lastGiveOrderTicks", LookMode.Def, LookMode.Value);
        }
    }
}
