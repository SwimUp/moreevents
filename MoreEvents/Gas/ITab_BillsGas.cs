using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimOverhaul.Gas
{
    public class ITab_BillsGas : ITab
    {
        private float viewHeight = 1000f;

        private Vector2 scrollPosition = default(Vector2);

        private Bill mouseoverBill;

        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        [TweakValue("Interface", 0f, 128f)]
        private static float PasteX = 48f;

        [TweakValue("Interface", 0f, 128f)]
        private static float PasteY = 3f;

        [TweakValue("Interface", 0f, 32f)]
        private static float PasteSize = 24f;

        protected Building_GasStation SelTable => (Building_GasStation)base.SelThing;

        public ITab_BillsGas()
        {
            size = WinSize;
            labelKey = "TabBills";
            tutorTag = "Bills";
        }

        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            Vector2 winSize = WinSize;
            Rect rect2 = new Rect(winSize.x - PasteX, PasteY, PasteSize, PasteSize);
            Vector2 winSize2 = WinSize;
            float x = winSize2.x;
            Vector2 winSize3 = WinSize;
            Rect rect3 = new Rect(0f, 0f, x, winSize3.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int i = 0; i < SelTable.def.AllRecipes.Count; i++)
                {
                    if (SelTable.def.AllRecipes[i].AvailableNow)
                    {
                        RecipeDef recipe = SelTable.def.AllRecipes[i];
                        list.Add(new FloatMenuOption(recipe.LabelCap, delegate
                        {
                            if (!SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
                            {
                                Bill.CreateNoPawnsWithSkillDialog(recipe);
                            }
                            Bill bill2 = new Bill_GasProduction(recipe, SelTable);
                            SelTable.billStack.AddBill(bill2);
                            if (recipe.conceptLearned != null)
                            {
                                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                            }
                            if (TutorSystem.TutorialMode)
                            {
                                TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
                            }
                        }, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, recipe)));
                    }
                }
                if (!list.Any())
                {
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), null));
                }
                return list;
            };
            mouseoverBill = SelTable.billStack.DoListing(rect3, recipeOptionsMaker, ref scrollPosition, ref viewHeight);
        }

        public override void TabUpdate()
        {
            if (mouseoverBill != null)
            {
                mouseoverBill.TryDrawIngredientSearchRadiusOnMap(SelTable.Position);
                mouseoverBill = null;
            }
        }
    }
}
