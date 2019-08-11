using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.CokeFurnace
{
    [StaticConstructorOnStartup]
    public class ITab_CokeFurnace : ITab
    {
        private Building_CokeFurnace furnace => (Building_CokeFurnace)base.SelThing;

        private Vector2 winSize => new Vector2(420, 480);

        public static Texture2D Background;

        public ITab_CokeFurnace()
        {
            size = winSize;
            labelKey = "TabCokeFurnace".Translate();
        }

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0, 0, winSize.x, winSize.y);

            GUI.DrawTexture(mainRect, Background);

            Rect buttonRect = new Rect(10, 25, mainRect.width - 20, 30);
            if (Widgets.ButtonText(buttonRect, "CheckRecipes".Translate(furnace.SelectedRecipe.products[0].thingDef.LabelCap)))
            {
                if(furnace.Started)
                {
                    Messages.Message("CokeFurnace_Started".Translate(), MessageTypeDefOf.NeutralEvent, false);
                    return;
                }

                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var recipe in furnace.Recipes)
                {
                    if (recipe.researchPrerequisite != null && Find.ResearchManager.GetProgress(recipe.researchPrerequisite) < recipe.researchPrerequisite.baseCost)
                        continue;

                    options.Add(new FloatMenuOption(recipe.products[0].thingDef.LabelCap, delegate
                    {
                        furnace.SelectNewRecipe(recipe);
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect titleRect = new Rect(10, 60, 400, 25);
            Widgets.Label(titleRect, "CockeFurnace_Ingedients".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            Rect leftRect = new Rect(10, titleRect.y + 35, 80, 80);
            foreach (var ingredient in furnace.SelectedRecipe.ingredients)
            {
                Widgets.DrawAtlas(leftRect, Widgets.ButtonSubtleAtlas);

                Rect ingRect = new Rect(leftRect.x + 10, leftRect.y + 10, 60, 60);
                GUI.DrawTexture(ingRect, ingredient.FixedIngredient.uiIcon);

                Widgets.DrawHighlightIfMouseover(leftRect);

                Rect second = new Rect(leftRect.x, leftRect.y + 85, 80, 30);

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(second, $"{furnace.ContainedResources[ingredient.FixedIngredient]} / {ingredient.GetBaseCount()}");
                Text.Anchor = TextAnchor.UpperLeft;

                leftRect.y += 120;
            }

            if(furnace.Result != null)
            {
                Widgets.Label(new Rect(320, 170, 80, 25), "CokeFurnace_TakeItem".Translate());
            }

            Rect resultRect = new Rect(320, 198, 80, 80);
            Widgets.DrawAtlas(resultRect, Widgets.ButtonSubtleAtlas);
            Widgets.DrawHighlightIfMouseover(resultRect);
            GUI.DrawTexture(new Rect(resultRect.x + 10, resultRect.y + 10, 60, 60), furnace.SelectedRecipe.products[0].thingDef.uiIcon);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(resultRect.x, resultRect.y + 85, 80, 30), $"{furnace.SelectedRecipe.products[0].count}");
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
