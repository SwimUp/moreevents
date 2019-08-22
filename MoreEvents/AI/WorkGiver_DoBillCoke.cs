using MoreEvents;
using RimOverhaul.Things.CokeFurnace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimOverhaul.AI
{
    public class WorkGiver_DoBillCoke : WorkGiver_Scanner
    {
        private class DefCountList
        {
            private List<ThingDef> defs = new List<ThingDef>();

            private List<float> counts = new List<float>();

            public int Count => defs.Count;

            public float this[ThingDef def]
            {
                get
                {
                    int num = defs.IndexOf(def);
                    if (num < 0)
                    {
                        return 0f;
                    }
                    return counts[num];
                }
                set
                {
                    int num = defs.IndexOf(def);
                    if (num < 0)
                    {
                        defs.Add(def);
                        counts.Add(value);
                        num = defs.Count - 1;
                    }
                    else
                    {
                        counts[num] = value;
                    }
                    CheckRemove(num);
                }
            }

            public float GetCount(int index)
            {
                return counts[index];
            }

            public void SetCount(int index, float val)
            {
                counts[index] = val;
                CheckRemove(index);
            }

            public ThingDef GetDef(int index)
            {
                return defs[index];
            }

            private void CheckRemove(int index)
            {
                if (counts[index] == 0f)
                {
                    counts.RemoveAt(index);
                    defs.RemoveAt(index);
                }
            }

            public void Clear()
            {
                defs.Clear();
                counts.Clear();
            }

            public void GenerateFrom(List<Thing> things)
            {
                Clear();
                for (int i = 0; i < things.Count; i++)
                {
                    DefCountList defCountList;
                    ThingDef def;
                    (defCountList = this)[def = things[i].def] = defCountList[def] + (float)things[i].stackCount;
                }
            }
        }

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        private List<ThingCount> chosenIngThings = new List<ThingCount>();

        private static List<Thing> relevantThings = new List<Thing>();

        private static HashSet<Thing> processedThings = new HashSet<Thing>();

        private static List<Thing> newRelevantThings = new List<Thing>();

        private static List<IngredientCount> ingredientsOrdered = new List<IngredientCount>();

        private static DefCountList availableCounts = new DefCountList();

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                if (def.fixedBillGiverDefs != null && def.fixedBillGiverDefs.Count == 1)
                {
                    return ThingRequest.ForDef(def.fixedBillGiverDefs[0]);
                }
                return ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            Building_CokeFurnace furnace = thing as Building_CokeFurnace;
            if (furnace != null && !furnace.Started)
            {
                LocalTargetInfo target = thing;
                bool ignoreOtherReservations = forced;
                if (pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations) && !thing.IsBurning() && !thing.IsForbidden(pawn))
                {
                    if (furnace.Result != null)
                    {
                        return new Job(JobDefOfLocal.TakeResultFromCokeFurnace, furnace);
                    }
                    if (furnace.Ready)
                    {
                        return TryStartFurnace(pawn, furnace);
                    }

                    CompRefuelable compRefuelable = thing.TryGetComp<CompRefuelable>();
                    if (compRefuelable != null && !compRefuelable.HasFuel)
                    {
                        if (!RefuelWorkGiverUtility.CanRefuel(pawn, thing, forced))
                        {
                            return null;
                        }
                        return RefuelWorkGiverUtility.RefuelJob(pawn, thing, forced);
                    }
                    if (furnace.ProduceCount > 0 || furnace.Infinity)
                    {
                        return TryStartJob(pawn, furnace);
                    }
                }
            }
            return null;
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Some;
        }

        private Job TryStartJob(Pawn pawn, Building_CokeFurnace furnace)
        {
            SkillRequirement skillRequirement = furnace.SelectedRecipe.FirstSkillRequirementPawnDoesntSatisfy(pawn);
            if (skillRequirement != null)
            {
                JobFailReason.Is("UnderRequiredSkill".Translate(skillRequirement.minLevel), furnace.Label);
                return null;
            }

            if (!TryFindBestBillIngredients(pawn, furnace, chosenIngThings))
            {
                JobFailReason.Is("MissingMaterials".Translate(), furnace.Label);
                chosenIngThings.Clear();
                return null;
            }
            Job result = TryStartNewDoBillJob(pawn, furnace);
            chosenIngThings.Clear();
            return result;
        }

        private Job TryStartNewDoBillJob(Pawn pawn, Building_CokeFurnace furnace)
        {
            Job job = new Job(JobDefOfLocal.CarryItemToCokeFurnace, furnace);
            job.countQueue = new List<int>(chosenIngThings.Count);
            job.targetQueueB = new List<LocalTargetInfo>(chosenIngThings.Count);

            for (int i = 0; i < chosenIngThings.Count; i++)
            {
                job.targetQueueB.Add(chosenIngThings[i].Thing);
                job.countQueue.Add(chosenIngThings[i].Count);
            }
            return job;
        }

        private Job TryStartFurnace(Pawn pawn, Building_CokeFurnace furnace)
        {
            Job job = new Job(JobDefOfLocal.SwitchCokeFurnace, furnace);

            return job;
        }

        private static bool TryFindBestBillIngredients(Pawn pawn, Building_CokeFurnace billGiver, List<ThingCount> chosen)
        {
            chosen.Clear();
            newRelevantThings.Clear();
            if (billGiver.SelectedRecipe.ingredients.Count == 0)
            {
                return true;
            }
            IntVec3 rootCell = GetBillGiverRootCell(billGiver, pawn);
            Region rootReg = rootCell.GetRegion(pawn.Map);
            if (rootReg == null)
            {
                return false;
            }
            MakeIngredientsListInProcessingOrder(ingredientsOrdered, billGiver);
            relevantThings.Clear();
            processedThings.Clear();
            bool foundAll = false;
            Predicate<Thing> baseValidator = (Thing t) => t.Spawned && !t.IsForbidden(pawn) && (float)(t.Position - billGiver.Position).LengthHorizontalSquared < 9999f && pawn.CanReserve(t);
            TraverseParms traverseParams = TraverseParms.For(pawn);
            RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParams, isDestination: false);
            int adjacentRegionsAvailable = rootReg.Neighbors.Count((Region region) => entryCondition(rootReg, region));
            int regionsProcessed = 0;
            processedThings.AddRange(relevantThings);
            RegionProcessor regionProcessor = delegate (Region r)
            {
                List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
                for (int i = 0; i < list.Count; i++)
                {
                    Thing thing = list[i];
                    if (!processedThings.Contains(thing) && ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, pawn) && baseValidator(thing) && !thing.def.IsMedicine)
                    {
                        newRelevantThings.Add(thing);
                        processedThings.Add(thing);
                    }
                }
                regionsProcessed++;
                if (newRelevantThings.Count > 0 && regionsProcessed > adjacentRegionsAvailable)
                {
                    Comparison<Thing> comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.Position - rootCell).LengthHorizontalSquared;
                        float value = (t2.Position - rootCell).LengthHorizontalSquared;
                        return num.CompareTo(value);
                    };
                    newRelevantThings.Sort(comparison);
                    relevantThings.AddRange(newRelevantThings);
                    newRelevantThings.Clear();
                    if (TryFindBestBillIngredientsInSet(relevantThings, billGiver, chosen))
                    {
                        foundAll = true;
                        return true;
                    }
                }
                return false;
            };
            RegionTraverser.BreadthFirstTraverse(rootReg, entryCondition, regionProcessor, 99999);
            relevantThings.Clear();
            newRelevantThings.Clear();
            processedThings.Clear();
            ingredientsOrdered.Clear();
            return foundAll;
        }

        private static void MakeIngredientsListInProcessingOrder(List<IngredientCount> ingredientsOrdered, Building_CokeFurnace bill)
        {
            ingredientsOrdered.Clear();
            if (bill.SelectedRecipe.productHasIngredientStuff)
            {
                ingredientsOrdered.Add(bill.SelectedRecipe.ingredients[0]);
            }
            for (int i = 0; i < bill.SelectedRecipe.ingredients.Count; i++)
            {
                if (!bill.SelectedRecipe.productHasIngredientStuff || i != 0)
                {
                    IngredientCount ingredientCount = bill.SelectedRecipe.ingredients[i];
                    if (ingredientCount.IsFixedIngredient)
                    {
                        ingredientsOrdered.Add(ingredientCount);
                    }
                }
            }
            for (int j = 0; j < bill.SelectedRecipe.ingredients.Count; j++)
            {
                IngredientCount item = bill.SelectedRecipe.ingredients[j];
                if (!ingredientsOrdered.Contains(item))
                {
                    ingredientsOrdered.Add(item);
                }
            }
        }

        private static IntVec3 GetBillGiverRootCell(Building_CokeFurnace billGiver, Pawn forPawn)
        {
            if (billGiver != null)
            {
                if (billGiver.def.hasInteractionCell)
                {
                    return billGiver.InteractionCell;
                }
                Log.Error("Tried to find bill ingredients for " + billGiver + " which has no interaction cell.");
                return forPawn.Position;
            }
            return billGiver.Position;
        }

        private static bool TryFindBestBillIngredientsInSet(List<Thing> availableThings, Building_CokeFurnace bill, List<ThingCount> chosen)
        {
            if (bill.SelectedRecipe.allowMixingIngredients)
            {
                return TryFindBestBillIngredientsInSet_AllowMix(availableThings, bill, chosen);
            }
            return TryFindBestBillIngredientsInSet_NoMix(availableThings, bill, chosen);
        }
        private static bool TryFindBestBillIngredientsInSet_NoMix(List<Thing> availableThings, Building_CokeFurnace bill, List<ThingCount> chosen)
        {
            RecipeDef recipe = bill.SelectedRecipe;
            chosen.Clear();
            availableCounts.Clear();
            availableCounts.GenerateFrom(availableThings);
            for (int i = 0; i < ingredientsOrdered.Count; i++)
            {
                IngredientCount ingredientCount = recipe.ingredients[i];
                bool flag = false;
                for (int j = 0; j < availableCounts.Count; j++)
                {
                    float num = ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.SelectedRecipe);
                    if (num > availableCounts.GetCount(j) || !ingredientCount.filter.Allows(availableCounts.GetDef(j)) || !ingredientCount.IsFixedIngredient)
                    {
                        continue;
                    }
                    for (int k = 0; k < availableThings.Count; k++)
                    {
                        if (availableThings[k].def != availableCounts.GetDef(j))
                        {
                            continue;
                        }
                        int num2 = availableThings[k].stackCount - ThingCountUtility.CountOf(chosen, availableThings[k]);
                        if (num2 > 0)
                        {
                            int num3 = Mathf.Min(Mathf.FloorToInt(num), num2);
                            ThingCountUtility.AddToList(chosen, availableThings[k], num3);

                            num -= (float)num3;
                            if (num < 0.001f)
                            {
                                flag = true;
                                float count = availableCounts.GetCount(j);
                                count -= (float)ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.SelectedRecipe);
                                availableCounts.SetCount(j, count);
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TryFindBestBillIngredientsInSet_AllowMix(List<Thing> availableThings, Building_CokeFurnace bill, List<ThingCount> chosen)
        {
            chosen.Clear();
            for (int i = 0; i < bill.SelectedRecipe.ingredients.Count; i++)
            {
                IngredientCount ingredientCount = bill.SelectedRecipe.ingredients[i];
                float num = ingredientCount.GetBaseCount();
                for (int j = 0; j < availableThings.Count; j++)
                {
                    Thing thing = availableThings[j];
                    if (ingredientCount.filter.Allows(thing) && ingredientCount.IsFixedIngredient)
                    {
                        float num2 = bill.SelectedRecipe.IngredientValueGetter.ValuePerUnitOf(thing.def);
                        int num3 = Mathf.Min(Mathf.CeilToInt(num / num2), thing.stackCount);
                        ThingCountUtility.AddToList(chosen, thing, num3);
                        num -= (float)num3 * num2;
                        if (num <= 0.0001f)
                        {
                            break;
                        }
                    }
                }
                if (num > 0.0001f)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
