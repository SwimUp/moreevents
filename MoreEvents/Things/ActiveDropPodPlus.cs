using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things
{
    public class ActiveDropPodPlus : ActiveDropPod
    {
        public Action OpenCallback;

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);

            OpenCallback?.Invoke();
        }
    }
}
