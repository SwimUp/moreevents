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

        public static string buffer;

        private Vector2 winSize => new Vector2(420, 520);

        public static Texture2D Background;
        public static Texture2D BackgroundFillableBar;

        public ITab_CokeFurnace()
        {
            size = winSize;
            labelKey = "TabCokeFurnace".Translate();
        }

        public override void TabUpdate()
        {
            base.TabUpdate();
        }

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0, 0, winSize.x, winSize.y);

            GUI.DrawTexture(mainRect, Background);

            Rect buttonRect = new Rect(30, 25, mainRect.width - 60, 30);
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
            if (!furnace.Infinity)
            {
                Listing_Standard standart = new Listing_Standard();
                standart.Begin(new Rect(30, 60, mainRect.width - 175, 30));
                standart.IntEntry(ref furnace.ProduceCount, ref buffer);
                standart.End();
            }
            if (Widgets.ButtonText(new Rect(280, 60, 110, 24), furnace.Infinity ? "CokeFurnace_infinity".Translate() : "CokeFurnace_Notinfinity".Translate()))
            {
                furnace.Infinity = !furnace.Infinity;
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect titleRect = new Rect(22, 90, 380, 25);
            Widgets.Label(titleRect, "CockeFurnace_Ingedients".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            Rect leftRect = new Rect(50, titleRect.y + 35, 80, 80);
            foreach (var ingredient in furnace.SelectedRecipe.ingredients)
            {
                Rect ingRect = new Rect(leftRect.x + 10, leftRect.y + 10, 64, 64);
                GUI.DrawTexture(ingRect, ingredient.FixedIngredient.uiIcon);

                Widgets.DrawHighlightIfMouseover(ingRect);

                Rect second = new Rect(leftRect.x, leftRect.y + 85, 80, 30);

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(second, $"{furnace.ContainedResources[ingredient.FixedIngredient]} / {ingredient.GetBaseCount()}");
                Text.Anchor = TextAnchor.UpperLeft;

                leftRect.x += 120;

                TooltipHandler.TipRegion(ingRect, $"CokeFurnace_IngInfo".Translate(furnace.SelectedRecipe.products[0].thingDef.LabelCap, ingredient.FixedIngredient.LabelCap, ingredient.GetBaseCount(), furnace.ContainedResources[ingredient.FixedIngredient]));
            }

            if(furnace.Result != null)
            {
                Widgets.Label(new Rect(180, 255, 64, 25), "CokeFurnace_TakeItem".Translate());
            }

            Rect resultRect = new Rect(180, 307, 64, 64);
            Widgets.DrawHighlightIfMouseover(resultRect);
            GUI.DrawTexture(resultRect, furnace.SelectedRecipe.products[0].thingDef.uiIcon);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(resultRect.x, resultRect.y + 85, 64, 30), $"{furnace.SelectedRecipe.products[0].count}");
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(resultRect, "CokeFurnace_ResultInfo".Translate(furnace.SelectedRecipe.products[0].thingDef.LabelCap, furnace.SelectedRecipe.products[0].count));

            Rect fillRect = new Rect(46, 435, 327, 51);
            DrawCustomFillableBar(fillRect, furnace.TicksRemaining, furnace.SelectedRecipe.workAmount, BackgroundFillableBar);
        }

        private void DrawCustomFillableBar(Rect mainRect, float fill, float maxRef, Texture2D fillTexture)
        {
            float width = mainRect.width * (fill / maxRef);
            float offset = mainRect.width - width;
            GUI.DrawTexture(new Rect(mainRect.x + offset, mainRect.y, width, mainRect.height), fillTexture);
        }
    }
}
