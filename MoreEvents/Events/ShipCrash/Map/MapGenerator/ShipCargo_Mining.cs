using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class ShipCargo_Mining : Ship_Cargo
    {
        public override CargoType PartType => CargoType.Mining;
        public override string TexturePath => @"Map/cargo_mining";

        public override string ExpandLabel => Translator.Translate("ShipCargo_Mining_ExpandLabel");

        public override string Description => Translator.Translate("ShipCargo_Mining_Description");

        private const int minSupply = 5;
        private const int maxSupply = 19;

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
                Dangerous_HiddenAttack dang = new Dangerous_HiddenAttack(Rand.Range(8000, 15000), dangerousLevel, owner, map, 30);
                main.parent.AllComps.Add(dang);
            }
            else
            {
                Dangerous_AlreadyStand dang = new Dangerous_AlreadyStand(dangerousLevel, owner, map, 4, 6);
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
                        if(thing.thingCategories.Contains(ThingCategoryDefOf.ResourcesRaw))
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

                int count = Rand.Range(1, 4);

                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);
                for (int i2 = 0; i2 < count; i2++)
                {
                    ThingDef item = items.RandomElement();

                    int maxCount = 30;
                    if (item.BaseMarketValue >= 5)
                        maxCount = 10;
                    if (item.BaseMarketValue > 8)
                        maxCount = 15;

                    int itemCount = Rand.Range(6, maxCount);
                    thingContainer.AddItem(item, itemCount);
                    dangerousLevel += (item.BaseMarketValue * itemCount) * 0.5f;
                }

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }
    }
}
