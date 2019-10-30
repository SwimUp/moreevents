using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class ShipCargo_Complex : Ship_Cargo
    {
        public override CargoType PartType => CargoType.Complex;
        public override string TexturePath => @"Map/cargo_complex";

        public override string ExpandLabel => Translator.Translate("ShipCargo_Complex_ExpandLabel");

        public override string Description => Translator.Translate("ShipCargo_Complex_Description");

        private int minSupply => int.Parse(settings.Parameters["ShipCargo_Complex_MinSupply"].Value);
        private int maxSupply => int.Parse(settings.Parameters["ShipCargo_Complex_MaxSupply"].Value);

        private bool dangerous = true;
        private float dangerousLevel;

        private ShipCrashWorker main;

        public override void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner)
        {
            this.main = main;

            GenerateItems(map);

            if (dangerous)
            {
                GenerateDangerous(owner, map);
            }
        }

        private void GenerateDangerous(Faction owner, Verse.Map map)
        {
            if (Rand.Chance(0.3f))
            {
                Dangerous_HiddenAttack dang = new Dangerous_HiddenAttack(Rand.Range(8000, 25000), dangerousLevel, owner, map, 35);
                dang.parent = main.parent;
                main.parent.AllComps.Add(dang);
            }
            else
            {
                Dangerous_AlreadyStand dang = new Dangerous_AlreadyStand(dangerousLevel, owner, map, 5, 10);
            }
        }

        private void GenerateItems(Verse.Map map)
        {
            List<ThingDef> items = new List<ThingDef>();

            foreach(var thing in DefDatabase<ThingDef>.AllDefs)
            {
                if(thing.category == ThingCategory.Item)
                {
                    if(thing.thingCategories != null)
                    {
                        if(thing.thingCategories.Contains(ThingCategoryDefOf.ResourcesRaw) ||
                            thing.thingCategories.Contains(ThingCategoryDefOf.Foods) ||
                            thing.thingCategories.Contains(ThingCategoryDefOf.Medicine))
                        {
                            items.Add(thing);
                        }
                    }
                }
            }

            if (items.Count == 0)
                return;

            int itemsCount = Rand.Range(minSupply, maxSupply);

            for (int i = 0; i < itemsCount; i++)
            {
                IntVec3 pos = map.AllCells.Where(vec => !vec.Fogged(map)).RandomElement();
                if (pos.IsValid)
                    continue;

                int count = Rand.Range(3, 7);

                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);
                for (int i2 = 0; i2 < count; i2++)
                {
                    ThingDef item = items.RandomElement();

                    int maxCount = 30;

                    if (item.FirstThingCategory == ThingCategoryDefOf.Medicine)
                        maxCount = 6;
                    else if (item.FirstThingCategory == ThingCategoryDefOf.ResourcesRaw)
                    {
                        if (item.BaseMarketValue >= 5)
                            maxCount = 15;
                        if (item.BaseMarketValue > 8)
                            maxCount = 10;
                    }

                    int itemCount = Rand.Range(3, maxCount);
                    thingContainer.AddItem(item, itemCount);
                    dangerousLevel += (item.BaseMarketValue * itemCount) * 0.3f;
                }

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }
    }
}
