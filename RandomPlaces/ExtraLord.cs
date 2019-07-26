using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RandomPlaces
{
    public abstract class ExtraLord : IExposable
    {
        public abstract Lord GetLord(Faction faction, Map map);

        public virtual void ExposeData()
        {

        }
    }
}
