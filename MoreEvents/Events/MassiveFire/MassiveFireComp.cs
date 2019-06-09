using MoreEvents.Events.MassiveFire;
using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.Comps
{
    public class MassiveFireComp : WorldObjectComp
    {
        private EventSettings settings => Settings.EventsSettings["MassiveFire"];

        public int LifeTime = 0;
        public bool UseLifeTime = false;
        public int FireLevel = 0;

        public MassiveFireMapSite site;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref LifeTime, "lifePartTime");
            Scribe_Values.Look(ref FireLevel, "FireLevel");
            Scribe_Values.Look(ref UseLifeTime, "UseTime");
        }

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);

            Start(10);
        }

        public void Start(int min, int max)
        {
            LifeTime = Rand.Range(min, max) * 60000;
            UseLifeTime = true;
        }

        public void Start(int days)
        {
            LifeTime = days * 60000;
            UseLifeTime = true;
        }

        public void Stop()
        {
            UseLifeTime = false;
        }

        public void Start()
        {
            if (LifeTime == 0)
                return;

            UseLifeTime = true;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(UseLifeTime)
            {
                LifeTime--;

                if(LifeTime <= 0.0f)
                {
                    CheckState();
                }
            }
        }

        private void CheckState()
        {
            Stop();

            if (FireLevel == 2)
            {
                site.BurnedIt();
                return;
            }

            FireLevel++;
            Start(10);

            if (Rand.Chance(0.4f))
                ForceNext();

            site.ResetMaterial();
        }

        private void ForceNext()
        {
            for (int i = 0; i < this.site.Candidates.Count; i++)
            {
                if (Find.WorldObjects.AnyMapParentAt(this.site.Candidates[i]))
                    this.site.Candidates.RemoveAt(i);
            }

            if (this.site.Candidates.Count == 0)
            {
                this.site.CreateHeathOfFire(this.site.RootMap);

                Find.WorldObjects.Remove(this.site);
                return;
            }

            int nextTile = this.site.Candidates.RandomElement();
            this.site.Candidates.Remove(nextTile);
            MassiveFireMapSite site = (MassiveFireMapSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.MassiveFireSite);
            site.Candidates = this.site.Candidates;
            site.Tile = nextTile;
            site.RootTile = this.site.RootTile;
            site.RootMap = this.site.RootMap;
            Find.WorldObjects.Add(site);
        }

        public override string CompInspectStringExtra()
        {
            return $"{Translator.Translate("FireSpreadIs")} {GenDate.TicksToDays(LifeTime).ToString("f2")}";
        }

    }
}
