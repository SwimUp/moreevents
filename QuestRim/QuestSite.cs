using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class QuestSite : MapParent
    {
        public override string Label => quest.PlaceLabel;

        public override Texture2D ExpandingIcon => quest.ExpandingIcon ?? def.ExpandingIconTexture;

        public Quest Quest => quest;

        private Quest quest;

        public override Material Material
        {
            get
            {
                if (cachedMat == null)
                {
                    cachedMat = MaterialPool.MatFrom(def.texture, ShaderDatabase.WorldOverlayTransparentLit, WorldMaterials.WorldObjectRenderQueue);
                }

                return cachedMat;
            }
        }
        private Material cachedMat = null;

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach(var opt in base.GetFloatMenuOptions(caravan))
            {
                yield return opt;
            }

            foreach(var opt in quest.GetFloatMenuOptions(caravan, this))
            {
                yield return opt;
            }
        }

        public override IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
        {
            foreach (var opt in base.GetTransportPodsFloatMenuOptions(pods, representative))
            {
                yield return opt;
            }

            foreach (var opt in quest.GetTransportPodsFloatMenuOptions(pods, representative))
            {
                yield return opt;
            }
        }

        public override string GetInspectString()
        {
            string text = quest.GetInspectString();
            return text;
        }

        public override void Tick()
        {
            base.Tick();

            quest.SiteTick();
        }

        public override string GetDescription()
        {
            return quest.GetDescription();
        }

        public void Init(Quest quest)
        {
            this.quest = quest;
        }

        public void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            quest.EndQuest(caravan, condition);

            Find.WorldObjects.Remove(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref quest, "Quest");
        }
    }
}
