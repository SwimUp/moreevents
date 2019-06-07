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
    public class MapParentLifeTimer : WorldObjectComp
    {
        private int lifeTime = 0;
        private bool useLifeTime = false;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref lifeTime, "lifePartTime");
        }

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);
        }

        public void Start(int min, int max)
        {
            lifeTime = Rand.Range(min, max) * 60000;
            useLifeTime = true;
        }

        public void Start(int days)
        {
            lifeTime = days * 60000;
            useLifeTime = true;
        }

        public void Stop()
        {
            useLifeTime = false;
            lifeTime = 0;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(useLifeTime)
            {
                lifeTime--;

                if(lifeTime <= 0.0f)
                {

                }
            }
        }

        public override string CompInspectStringExtra()
        {
            return $"{Translator.Translate("FireSpreadIs")} {(int)GenDate.TicksToDays(lifeTime)}";
        }

    }
}
