using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class Ship_Armory : ShipMapGenerator
    {
        private EventSettings settings => Settings.EventsSettings["ShipCrash"];

        public override ShipSiteType SiteType => ShipSiteType.Armory;

        public override string TexturePath => @"Map/armory";

        public override string ExpandLabel => Translator.Translate("Ship_Armory_ExpandLabel");

        public override string Description => Translator.Translate("Ship_Armory_Description");

        private ShipCrashWorker main;

        private int minSupply => int.Parse(settings.Parameters["ShipCargo_Armory_MinSupply"].Value);
        private int maxSupply => int.Parse(settings.Parameters["ShipCargo_Armory_MaxSupply"].Value);

        private float dangerousLevel = 0;

        public override void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner)
        {
            this.main = main;

            GenerateLoot(map);

            GenerateDangerous(map, owner);
        }

        private void GenerateDangerous(Verse.Map map, Faction owner)
        {
            if (Rand.Chance(0.23f))
            {
                Dangerous_HiddenFlyAttack dang = new Dangerous_HiddenFlyAttack(Rand.Range(3500, 10000), dangerousLevel, owner, map, Rand.Range(5, 10), Rand.Range(14,25), 30);
                main.parent.AllComps.Add(dang);
            }else if(Rand.Chance(0.32f))
            {
                Dangerous_HiddenAttack dang = new Dangerous_HiddenAttack(Rand.Range(8000, 10000), dangerousLevel, owner, map, 30);
                main.parent.AllComps.Add(dang);
            }
            else
            {
                Dangerous_AlreadyStand dang = new Dangerous_AlreadyStand(dangerousLevel, owner, map, 8, 10);
            }
        }

        private void GenerateLoot(Verse.Map map)
        {
            int itemsCount = Rand.Range(minSupply, maxSupply);

            for (int i = 0; i < itemsCount; i++)
            {
                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);

                IntVec3 pos = map.AllCells.Where(vec => !vec.Fogged(map)).RandomElement();
                if (pos == null)
                    continue;

                GenerateApparels(map, thingContainer);

                GenerateWeapons(map, thingContainer);

                if(thingContainer.items.Count == 0)
                {
                    if (Rand.Chance(0.20f))
                        GenerateWeapons(map, thingContainer);
                }

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }

        private void GenerateApparels(Verse.Map map, Building_Container thingContainer)
        {
            List<ThingDef> items = new List<ThingDef>();

            foreach (var thing in DefDatabase<ThingDef>.AllDefs)
            {
                if (thing.category == ThingCategory.Item)
                {
                    if (thing.thingCategories != null)
                    {
                        if (thing.thingCategories.Contains(ThingCategoryDefOf.Apparel))
                        {
                            items.Add(thing);
                        }
                    }
                }
            }

            if (items.Count == 0)
                return;

            int count = Rand.Range(0, 3);

            if (count == 0)
                return;

            for (int i2 = 0; i2 < count; i2++)
            {
                ThingDef item = items.RandomElement();
                Thing item2 = ThingMaker.MakeThing(item, GenStuff.RandomStuffFor(item));
                if (Rand.Chance(0.08f))
                {
                    item2.TryGetComp<CompQuality>()?.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
                }
                else
                {
                    int val = Rand.Range(0, 5);
                    item2.TryGetComp<CompQuality>()?.SetQuality((QualityCategory)val, ArtGenerationContext.Colony);
                }

                thingContainer.AddItem(item2);
                dangerousLevel += item2.MarketValue;
            }
        }

        private void GenerateWeapons(Verse.Map map, Building_Container thingContainer)
        {
            List<ThingDef> items = new List<ThingDef>();

            foreach (var thing in DefDatabase<ThingDef>.AllDefs)
            {
                if(thing.IsRangedWeapon || thing.IsMeleeWeapon)
                {
                    items.Add(thing);
                }
            }

            if (items.Count == 0)
                return;

            int count = Rand.Range(0, 3);

            if (count == 0)
                return;

            for (int i2 = 0; i2 < count; i2++)
            {
                ThingDef item = items.RandomElement();
                Thing item2 = ThingMaker.MakeThing(item, GenStuff.RandomStuffFor(item));

                if (Rand.Chance(0.08f))
                {
                    item2.TryGetComp<CompQuality>()?.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
                }
                else
                {
                    int val = Rand.Range(0, 5);
                    item2.TryGetComp<CompQuality>()?.SetQuality((QualityCategory)val, ArtGenerationContext.Colony);
                }

                thingContainer.AddItem(item2);
                dangerousLevel += item2.MarketValue;
            }
        }
    }
}
