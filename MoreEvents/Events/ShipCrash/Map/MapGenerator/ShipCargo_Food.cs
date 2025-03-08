﻿using System;
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
    //    public override string TexturePath => @"Map/cargo_food";

    //    public override string ExpandLabel => Translator.Translate("ShipCargo_Food_ExpandLabel");

    //    public override string Description => Translator.Translate("ShipCargo_Food_Description");

        private int minSupply => int.Parse(settings.Parameters["ShipCargo_Food_MinSupply"].Value);
        private int maxSupply => int.Parse(settings.Parameters["ShipCargo_Food_MaxSupply"].Value);

        private bool dangerous = true;
        private float dangerousLevel;

        private ShipCrashWorker main;

        public override void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner)
        {
            this.main = main;

            GenerateItems(map);

            if(dangerous)
            {
                if(dangerousLevel > 1400f)
                {
                    dangerousLevel *= 0.6f;
                    GenerateDangerous(owner, map);
                }
            }
        }

        private void GenerateDangerous(Faction owner, Verse.Map map)
        {
            Dangerous_AlreadyStand dang = new Dangerous_AlreadyStand(dangerousLevel, owner, map, 4, 4);
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

                int count = Rand.Range(2, 6);

                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);
                for (int i2 = 0; i2 < count; i2++)
                {
                    ThingDef item = items.RandomElement();

                    int itemCount = Rand.Range(6, 15);
                    thingContainer.AddItem(item, itemCount);
                    dangerousLevel += item.BaseMarketValue * itemCount;
                }

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }
    }
}
