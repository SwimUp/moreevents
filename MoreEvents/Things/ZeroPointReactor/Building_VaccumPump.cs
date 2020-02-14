using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.ZeroPointReactor
{
    public class Building_VaccumPump : Building
    {
        private Building_ZeroPointGenerator generator;
        private Room cachedRoom = null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            Notify_RoomChange();
        }

        public override void TickRare()
        {
            base.TickRare();

            if(cachedRoom == null)
            {
                if (!TryGetRoom())
                    return;
            }

            Room room = this.GetRoom(RegionType.Set_All);

            if (room != cachedRoom)
            {
                Notify_RoomChange();
            }
        }

        private bool TryGetRoom()
        {
            cachedRoom = this.GetRoom(RegionType.Set_All);

            if (cachedRoom == null || cachedRoom.Role == RoomRoleDefOf.None)
                return false;

            return true;
        }

        private void Notify_RoomChange()
        {
            cachedRoom = this.GetRoom(RegionType.Set_All);
            generator = null;

            if (cachedRoom == null || cachedRoom.Role == RoomRoleDefOf.None)
                return;

            Thing thing = cachedRoom.ContainedAndAdjacentThings.Where(t => t is Building_ZeroPointGenerator).FirstOrDefault();
            if(thing != null)
            {
                generator = (Building_ZeroPointGenerator)thing;
                generator.Notify_PumpsChange();
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);

            if (generator != null)
            {
                generator.Notify_PumpsChange();
            }
        }
    }
}
