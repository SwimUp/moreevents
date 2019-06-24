using MoreEvents.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents
{
    public static class SkyfallerPlusMaker
    {
        public static SkyfallerPlus MakeSkyfaller(ThingDef skyfaller, Action impactAction = null)
        {
            SkyfallerPlus faller = (SkyfallerPlus)ThingMaker.MakeThing(skyfaller);
            faller.ImpactAction = impactAction;

            return faller;
        }

        public static SkyfallerPlus MakeSkyfaller(ThingDef skyfaller, ThingDef innerThing, Action impactAction = null)
        {
            Thing innerThing2 = ThingMaker.MakeThing(innerThing);
            return MakeSkyfaller(skyfaller, innerThing2, impactAction);
        }

        public static SkyfallerPlus MakeSkyfaller(ThingDef skyfaller, Thing innerThing, Action impactAction = null)
        {
            SkyfallerPlus skyfaller2 = MakeSkyfaller(skyfaller, impactAction);
            if (innerThing != null && !skyfaller2.innerContainer.TryAdd(innerThing))
            {
                Log.Error("Could not add " + innerThing.ToStringSafe() + " to a skyfaller.");
                innerThing.Destroy();
            }
            return skyfaller2;
        }

        public static SkyfallerPlus MakeSkyfaller(ThingDef skyfaller, IEnumerable<Thing> things, Action impactAction = null)
        {
            SkyfallerPlus skyfaller2 = MakeSkyfaller(skyfaller, impactAction);
            if (things != null)
            {
                skyfaller2.innerContainer.TryAddRangeOrTransfer(things, canMergeWithExistingStacks: false, destroyLeftover: true);
            }
            return skyfaller2;
        }

        public static SkyfallerPlus SpawnSkyfaller(ThingDef skyfaller, IntVec3 pos, Map map, Action impactAction = null)
        {
            SkyfallerPlus newThing = MakeSkyfaller(skyfaller, impactAction);
            return (SkyfallerPlus)GenSpawn.Spawn(newThing, pos, map);
        }

        public static SkyfallerPlus SpawnSkyfaller(ThingDef skyfaller, ThingDef innerThing, IntVec3 pos, Map map, Action impactAction = null)
        {
            SkyfallerPlus newThing = MakeSkyfaller(skyfaller, innerThing, impactAction);
            return (SkyfallerPlus)GenSpawn.Spawn(newThing, pos, map);
        }

        public static SkyfallerPlus SpawnSkyfaller(ThingDef skyfaller, Thing innerThing, IntVec3 pos, Map map, Action impactAction = null)
        {
            SkyfallerPlus newThing = MakeSkyfaller(skyfaller, innerThing, impactAction);
            return (SkyfallerPlus)GenSpawn.Spawn(newThing, pos, map);
        }

        public static SkyfallerPlus SpawnSkyfaller(ThingDef skyfaller, IEnumerable<Thing> things, IntVec3 pos, Map map, Action impactAction = null)
        {
            SkyfallerPlus newThing = MakeSkyfaller(skyfaller, things, impactAction);
            return (SkyfallerPlus)GenSpawn.Spawn(newThing, pos, map);
        }
    }
}
