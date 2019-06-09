using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map
{
    public class ShipCrashWorker : WorldObjectComp
    {
        public ShipSiteType SiteType => Generator.SiteType;

        public ShipMapGenerator Generator { get; private set; }

        private int lifeTime = 0;
        private bool useLifeTime = false;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref lifeTime, "lifePartTime");
            Scribe_Values.Look(ref useLifeTime, "UseLifeTime");
        }

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);
            lifeTime = Rand.Range(10, 40) * 60000;
            useLifeTime = true;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(useLifeTime)
            {
                lifeTime--;

                if(lifeTime <= 0.0f)
                {
                    DestroyShipPart();
                }
            }
        }

        private void DestroyShipPart()
        {
            MapParent mapParent = (MapParent)parent;

            if(mapParent.HasMap)
            {
                ShipSite.ForceReform(mapParent);
            }
            else
            {
                Find.WorldObjects.Remove(mapParent);
            }
        }

        public override string CompInspectStringExtra()
        {
            return $"{Translator.Translate("ShipPartTimeout")} {(int)GenDate.TicksToDays(lifeTime)}";
        }

        public void InitWorker(ShipMapGenerator generator)
        {
            Generator = generator;
        }

        public override void PostMapGenerate()
        {
            Generator.RunGenerator(this, (parent as MapParent).Map, parent.Faction);
        }
    }
}
