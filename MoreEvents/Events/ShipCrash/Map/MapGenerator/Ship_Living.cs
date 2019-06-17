using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class Ship_Living : ShipMapGenerator
    {
        private EventSettings settings => Settings.EventsSettings["ShipCrash"];

        public override ShipSiteType SiteType => ShipSiteType.Living;

        public override string TexturePath => @"Map/living";

        public override string ExpandLabel => Translator.Translate("Ship_Living_ExpandLabel");

        public override string Description => Translator.Translate("Ship_Living_Description");

        private ShipCrashWorker main;

        private int minSupply => int.Parse(settings.Parameters["ShipCargo_Living_MinSupply"].Value);
        private int maxSupply => int.Parse(settings.Parameters["ShipCargo_Living_MaxSupply"].Value);

        private float dangerousLevel;

        public override void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner)
        {
            this.main = main;

            GenerateLoot(map, owner);

            GenerateDangerous(map, owner);
        }

        private void GenerateDangerous(Verse.Map map, Faction owner)
        {
            if (dangerousLevel < 3000f)
                dangerousLevel *= 1.24f;

            if (Rand.Chance(0.5f))
            {
                Dangerous_HiddenAttack dang = new Dangerous_HiddenAttack(Rand.Range(7500, 14000), dangerousLevel, owner, map, 20);
                dang.parent = main.parent;
                main.parent.AllComps.Add(dang);
            }
            else
            {
                Dangerous_AlreadyStand dang = new Dangerous_AlreadyStand(dangerousLevel, owner, map, 5, 8);
            }
        }

        private void GenerateLoot(Verse.Map map, Faction owner)
        {
            List<ThingDef> items = new List<ThingDef>();

            foreach (var thing in DefDatabase<ThingDef>.AllDefs)
            {
                if (thing.category == ThingCategory.Item)
                {
                    if (thing.thingCategories != null)
                    {
                        if (thing.thingCategories.Contains(ThingCategoryDefOf.Foods) ||
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
                if (pos == null)
                    continue;

                Building_Container thingContainer = (Building_Container)ThingMaker.MakeThing(ThingDefOfLocal.OpenableContainer);
                GenerateItems(thingContainer, items);
                GeneratePawns(thingContainer, owner);

                GenSpawn.Spawn(thingContainer, pos, map);
            }
        }

        private void GeneratePawns(Building_Container container, Faction owner)
        {
            int pawnsCount = Rand.Range(1, 4);

            for(int i = 0; i < pawnsCount; i++)
            {
                Pawn p = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Colonist, owner));

                if (Rand.Chance(0.2f))
                    HealthUtility.DamageUntilDead(p);
                else
                    HealthUtility.DamageUntilDowned(p, false);

                container.AddItem(p);

                dangerousLevel += p.MarketValue * Rand.Range(1f, 1.7f);
            }
        }

        private void GenerateItems(Building_Container container, List<ThingDef> items)
        {
            int count = Rand.Range(1, 3);

            for (int i2 = 0; i2 < count; i2++)
            {
                ThingDef item = items.RandomElement();

                int maxCount = 25;

                if (item.FirstThingCategory == ThingCategoryDefOf.Medicine)
                    maxCount = 15;

                int itemCount = Rand.Range(1, maxCount);
                container.AddItem(item, itemCount);
                dangerousLevel += (item.BaseMarketValue * itemCount) * 0.14f;
            }
        }
    }
}
