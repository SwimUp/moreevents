using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimOverhaul.Things.CokeFurnace
{
    [StaticConstructorOnStartup]
    public class Building_CokeFurnace : Building
    {
        public RecipeDef SelectedRecipe;

        public Thing Result => result;
        private Thing result;

        public IEnumerable<RecipeDef> Recipes => recipes;
        private List<RecipeDef> recipes = new List<RecipeDef>();

        public Dictionary<ThingDef, int> ContainedResources;

        public bool Started => started;
        private bool started = false;

        public float TicksRemaining => ticksRemaining;
        private float ticksRemaining = 0;

        public string GraphicOnPath => base.def.graphic.path + "_on";

        private Graphic graphic;

        private CompRefuelable refuelableComp;

        private float consumeWhenActive = 0f;
        private float consumeWhenInactive = 0;

        public bool Ready => refuelableComp.HasFuel && IngredientsReady;

        public bool IngredientsReady => !SelectedRecipe.ingredients.Any(x => x.GetBaseCount() != ContainedResources[x.FixedIngredient]);

        public int ProduceCount = 0;
        public bool Infinity = false;
        public string buffer;

        static Building_CokeFurnace()
        {
            ITab_CokeFurnace.Background = ContentFinder<Texture2D>.Get("UI/CokeFurnaceBack");
            ITab_CokeFurnace.BackgroundFillableBar = ContentFinder<Texture2D>.Get("UI/furnacebackgroundback");
        }

        public void SelectNewRecipe(RecipeDef recipe)
        {
            if(ContainedResources == null)
                ContainedResources = new Dictionary<ThingDef, int>();

            SelectedRecipe = recipe;

            DropContainedResources();

            foreach(var ingredient in recipe.ingredients)
            {
                ContainedResources.Add(ingredient.FixedIngredient, 0);
            }
        }

        public void DropResult()
        {
            if (result == null)
                return;

            GenDrop.TryDropSpawn(result, InteractionCell, Map, ThingPlaceMode.Near, out Thing resultingThing);
            //GenSpawn.Spawn(result, InteractionCell, Map);
            result = null;

            if (!Infinity)
            {
                ProduceCount = Mathf.Clamp(ProduceCount - 1, 0, ProduceCount);
                buffer = ProduceCount.ToString();
            }
        }

        public void DropContainedResources()
        {
            foreach (var resource in ContainedResources)
            {
                if (resource.Value > 0)
                {
                    if (CellFinder.TryFindRandomCellNear(Position, Map, 4, null, out IntVec3 result))
                    {
                        Thing t = ThingMaker.MakeThing(resource.Key);
                        t.stackCount = resource.Value;

                        GenDrop.TryDropSpawn(t, result, Map, ThingPlaceMode.Near, out Thing resultingThing);
                        //GenSpawn.Spawn(t, result, Map);
                    }
                }
            }

            ContainedResources.Clear();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            recipes = def.recipes;

            base.SpawnSetup(map, respawningAfterLoad);

            if(SelectedRecipe == null)
                SelectNewRecipe(Recipes.RandomElement());

            refuelableComp = GetComp<CompRefuelable>();

            consumeWhenActive = refuelableComp.Props.fuelConsumptionRate / 60000f;
            consumeWhenInactive = consumeWhenActive / 3;

            LongEventHandler.ExecuteWhenFinished((Action)FindTexture);
        }

        public void FindTexture()
        {
            Texture2D val = ContentFinder<Texture2D>.Get(GraphicOnPath + "_north", false);

            graphic = GraphicDatabase.Get<Graphic_Multi>(GraphicOnPath, base.def.graphic.Shader, base.def.graphic.drawSize, base.def.graphic.Color);
        }

        public void AddItemToFurnace(Thing item)
        {
            if(ContainedResources.ContainsKey(item.def))
            {
                IngredientCount ingr = SelectedRecipe.ingredients.Where(x => x.FixedIngredient == item.def).FirstOrDefault();

                int remaining = (int)ingr.GetBaseCount() - ContainedResources[item.def];
                int toSplit = Mathf.Min(item.stackCount, remaining);
                item.SplitOff(toSplit).Destroy();

                ContainedResources[item.def] += toSplit;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref ContainedResources, "Resources", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look(ref SelectedRecipe, "SelectedRecipe");
            Scribe_Values.Look(ref ticksRemaining, "ticks");
            Scribe_Values.Look(ref started, "started");
            Scribe_Deep.Look(ref result, "result");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (!started)
            {
                if(result != null)
                {
                    yield return new FloatMenuOption("CokeFurnace_TakeItem2".Translate(result.LabelCap), delegate
                    {
                        Job job = new Job(JobDefOfLocal.TakeResultFromCokeFurnace, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }

                bool canFire = CheckFurnace(out string reason);
                if (canFire)
                {
                    yield return new FloatMenuOption("CokeFurnace_StartFire".Translate(), delegate
                    {
                        Job job = new Job(JobDefOfLocal.SwitchCokeFurnace, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
                else
                {
                    yield return new FloatMenuOption($"{"CokeFurnace_StartFire".Translate()}{reason}", null);
                }

                if (ProduceCount > 0)
                {
                    foreach (var item in ContainedResources)
                    {
                        int baseCount = (int)SelectedRecipe.ingredients.Where(x => x.FixedIngredient == item.Key).FirstOrDefault().GetBaseCount();
                        if (item.Value < baseCount)
                        {
                            int count = baseCount - item.Value;
                            List<ThingCount> items = new List<ThingCount>();
                            bool hasItem = TryFindBestItem(selPawn, item.Key, count, items);
                            string label = "CokeFurnace_CarryItem".Translate(item.Key, count);
                            if (!hasItem)
                                label += "CokeFurnace_CarryItem_NoItem".Translate();

                            var option = new FloatMenuOption(label, null);
                            if (hasItem)
                            {
                                option.action = delegate
                                {
                                    Job job = new Job(JobDefOfLocal.CarryItemToCokeFurnace, this);
                                    job.countQueue = new List<int>(items.Count);
                                    job.targetQueueB = new List<LocalTargetInfo>(items.Count);
                                    foreach (var toAdd in items)
                                    {
                                        job.countQueue.Add(toAdd.Count);
                                        job.targetQueueB.Add(toAdd.Thing);
                                    }
                                    selPawn.jobs.TryTakeOrderedJob(job);
                                };
                            }
                            yield return option;
                        }
                    }
                }
            }
            else
            {
                yield return new FloatMenuOption("CokeFurnace_off".Translate(), delegate
                {
                    Job job = new Job(JobDefOfLocal.SwitchCokeFurnace, this);
                    selPawn.jobs.TryTakeOrderedJob(job);
                });
            }
        }

        public void StartCoke()
        {
            ticksRemaining = SelectedRecipe.workAmount;

            started = true;
        }

        public void StopCoke()
        {
            ticksRemaining = 0;

            started = false;

            if (result == null)
            {
                foreach (var item in SelectedRecipe.ingredients)
                {
                    ContainedResources[item.FixedIngredient] = ContainedResources[item.FixedIngredient] / Rand.Range(2, 3);
                }
            }
            else
            {
                foreach (var item in SelectedRecipe.ingredients)
                {
                    ContainedResources[item.FixedIngredient] = 0;
                }
            }
        }

        private bool CheckFurnace(out string reason)
        {
            reason = string.Empty;

            foreach (var item in ContainedResources)
            {
                int baseCount = (int)SelectedRecipe.ingredients.Where(x => x.FixedIngredient == item.Key).FirstOrDefault().GetBaseCount();
                if (item.Value < baseCount)
                {
                    reason += "CokeFurnace_NoIng".Translate();
                    break;
                }
            }

            if (!refuelableComp.HasFuel)
            {
                reason += "CokeFurnace_NoFuel".Translate();
            }

            return string.IsNullOrEmpty(reason);
        }

        private bool TryFindBestItem(Pawn pawn, ThingDef thingDef, int need, List<ThingCount> chosen)
        {
            chosen.Clear();

            Region rootReg = Position.GetRegion(pawn.Map);
            if (rootReg == null)
            {
                return false;
            }

            Predicate<Thing> baseValidator = (Thing t) => t.Spawned && !t.IsForbidden(pawn) && pawn.CanReserve(t);
            TraverseParms traverseParams = TraverseParms.For(pawn);
            RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParams, isDestination: false);
            int adjacentRegionsAvailable = rootReg.Neighbors.Count((Region region) => entryCondition(rootReg, region));
            bool found = false;
            RegionProcessor regionProcessor = delegate (Region r)
            {
                List<Thing> list = r.ListerThings.ThingsOfDef(thingDef);
                for (int i = 0; i < list.Count; i++)
                {
                    Thing thing = list[i];

                    int count = Mathf.Min(thing.stackCount, need);
                    chosen.Add(new ThingCount(thing, count));
                    need -= count;

                    if (need <= 0)
                    {
                        found = true;
                        return true;
                    }

                }
                return false;
            };
            RegionTraverser.BreadthFirstTraverse(rootReg, entryCondition, regionProcessor, 99999);

            return found;
        }

        public override void Tick()
        {
            if (!refuelableComp.HasFuel)
                return;

            if (!started)
            {
                refuelableComp.ConsumeFuel(consumeWhenInactive);
                return;
            }

            base.Tick();

            ticksRemaining--;
            refuelableComp.ConsumeFuel(consumeWhenActive);

            if (ticksRemaining <= 0)
            {
                FinishCoke();
            }
        }

        public void FinishCoke()
        {
            if (result != null && result.def == SelectedRecipe.products[0].thingDef)
            {
                result.stackCount += SelectedRecipe.products[0].count;
            }
            else
            {
                result = ThingMaker.MakeThing(SelectedRecipe.products[0].thingDef);
                result.stackCount = SelectedRecipe.products[0].count;
            }

            StopCoke();
        }

        public override string GetInspectString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.GetInspectString());
            if(Started)
            {
                builder.Append("CokeFurnace_Active".Translate(((int)ticksRemaining).TicksToDays().ToString("f2")));
            }
            else
            {
                builder.Append("CokeFurnace_Inactive".Translate());
            }
            return builder.ToString();
        }

        public override void Draw()
        {
            base.Draw();
            if (!started)
            {
                return;
            }

            if (graphic != null)
            {
                Graphic obj = GraphicColoredFor(this, graphic);
                Mesh val2 = obj.MeshAt(Rotation);
                Material val3 = obj.MatAt(Rotation, null);
                if (base.def.graphic is Graphic_Multi)
                {
                    Graphics.DrawMesh(val2, this.DrawPos + Altitudes.AltIncVect, Quaternion.identity, val3, 0);
                }
            }
        }

        private Graphic GraphicColoredFor(Thing t, Graphic graphic)
        {
            if (GenColor.IndistinguishableFrom(t.DrawColor, graphic.Color) && GenColor.IndistinguishableFrom(t.DrawColorTwo, graphic.ColorTwo))
            {
                return graphic;
            }
            return graphic.GetColoredVersion(t.def.graphic.Shader, t.DrawColor, t.DrawColorTwo);
        }

    }
}
