using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things
{
    public class DropPodIncomingPlus : SkyfallerPlus, IActiveDropPod, IThingHolder
    {
        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveDropPod)innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveDropPod)innerContainer[0]).Contents = value;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }

        protected override void Impact()
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f);
                MoteMaker.ThrowDustPuff(loc, base.Map, 1.2f);
            }
            MoteMaker.ThrowLightningGlow(base.Position.ToVector3Shifted(), base.Map, 2f);
            GenClamor.DoClamor(this, 15f, ClamorDefOf.Impact);
            base.Impact();
        }
    }
}
