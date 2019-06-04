using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class ShipCargo_Food : Ship_Cargo
    {
        public override CargoType PartType => CargoType.Food;
        public override string texturePath => @"Map/cargo_food";

        public override string ExpandLabel => Translator.Translate("ShipCargo_Food_ExpandLabel");

        public override string Description => Translator.Translate("ShipCargo_Food_Description");

        private const int minSupply = 5;
        private const int maxSupply = 19;

        private bool dangerous = true;

        public override void RunGenerator(Verse.Map map)
        {
            GenerateItems(map);
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
                        if(thing.thingCategories.Contains(ThingCategoryDefOf.FoodMeals) || thing.thingCategories.Contains(ThingCategoryDefOf.Foods))
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
                if (pos == null)
                    continue;

                int count = Rand.Range(1, 6);

                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);
                for (int i2 = 0; i2 < count; i2++)
                {
                    ThingDef item = items.RandomElement();

                    int itemCount = Rand.Range(6, 35);
                    thingContainer.AddItem(item, itemCount);
                }

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }
    }
}
