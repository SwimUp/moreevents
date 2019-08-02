using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimArmorCore.Mk1
{
    public class ArmorModuleWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(750, 500);

        private Vector2 slider = Vector2.zero;
        private Vector2 slider2 = Vector2.zero;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
        private static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;

        private MKStationWindow mkStationWindow;
        private ArmorModuleDef currentModule;

        private Dictionary<ArmorModuleDef, string> canUse = new Dictionary<ArmorModuleDef, string>();

        private ArmorModuleCategory tab;

        public ArmorModuleWindow(MKStationWindow mKStationWindow, ArmorModuleCategory category)
        {
            doCloseX = true;

            tab = category;

            mkStationWindow = mKStationWindow;

            canUse = new Dictionary<ArmorModuleDef, string>();
            foreach(var module in DefDatabase<ArmorModuleDef>.AllDefs)
            {
                if(!CanUseRightNow(module.Item, mkStationWindow.mkStation.Map, module, out string reason))
                {
                    canUse.Add(module, reason);
                }
            }
        }

        public bool CanUseRightNow(ThingDef searchedItem, Map map, ArmorModuleDef def, out string reason)
        {
            reason = string.Empty;

            if (searchedItem == null)
            {
                reason = "Item not found";
                return false;
            }

            List<SlotGroup> allGroupsListForReading = map.haulDestinationManager.AllGroupsListForReading;
            bool found = false;
            for (int i = 0; i < allGroupsListForReading.Count; i++)
            {
                SlotGroup slotGroup = allGroupsListForReading[i];
                foreach (var item in slotGroup.HeldThings)
                {
                    if (item.def == searchedItem)
                    {
                        found = true;
                    }
                }
            }

            if (!found)
            {
                reason += $"\n{"Station_ModuleItemNotFound".Translate()}";
            }

            if(searchedItem.recipeMaker != null && searchedItem.recipeMaker.researchPrerequisite != null)
            {
                float project = Find.ResearchManager.GetProgress(searchedItem.recipeMaker.researchPrerequisite);
                if(project != searchedItem.recipeMaker.researchPrerequisite.baseCost)
                {
                    reason += $"\n{"Station_NeedResearch".Translate(searchedItem.recipeMaker.researchPrerequisite.LabelCap)}";
                }
            }

            if(mkStationWindow.mkStation.ContainedArmor != null)
            {
                if (def.StationLevelRequired > mkStationWindow.mkStation.StationLevel)
                {
                    reason += $"\n{"Station_LevelReq".Translate(def.StationLevelRequired, mkStationWindow.mkStation.StationLevel)}";
                }
            }

            return string.IsNullOrEmpty(reason);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord(tab.GetLabel(), delegate
            {
            }, true));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList);
            tabsList.Clear();

            DrawModuleTab(rect2, tab);
        }

        private void DrawModuleTab(Rect inRect, ArmorModuleCategory selectCategory)
        {
            Rect scrollVertRectFact = new Rect(0, 0, inRect.x, DefDatabase<MKStationModuleDef>.DefCount * 35);

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineVertical(245, inRect.y, 465);
            Widgets.DrawLineHorizontal(0, inRect.y, inRect.width);
            GUI.color = Color.white;

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect rect2 = new Rect(inRect.x + 10, 0, 230, 30);
            Rect sliderRect = new Rect(inRect.x, inRect.y + 10, inRect.width, inRect.height);
            Widgets.BeginScrollView(sliderRect, ref slider, scrollVertRectFact, true);
            foreach (var module in DefDatabase<ArmorModuleDef>.AllDefsListForReading.Where(x => x.ModuleCategory == selectCategory))
            {
                Color bColor = canUse.ContainsKey(module) ? Color.gray : Color.white;
                if (DrawCustomButton(rect2, module.LabelCap, bColor))
                {
                    currentModule = module;
                }
                rect2.y += 35;
            }
            Widgets.EndScrollView();
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect3 = new Rect(inRect.x + 240, inRect.y + 10, 470, 455);
            if(currentModule != null)
            {
                DrawModuleInfo(rect3, currentModule);
            }
        }

        private void DrawModuleInfo(Rect rect, ArmorModuleDef module)
        {
            Rect textureRect = new Rect(rect.x + (rect.x / 2) + 50, rect.y + 30, 100, 100);
            GUI.DrawTexture(textureRect, module.IconImage);

            Rect titleRect = new Rect(rect.x, rect.y, rect.width, 25);
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, module.LabelCap);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect mainRect = new Rect(rect.x + 15, textureRect.y + 115, 470, 300);
            Widgets.LabelScrollable(mainRect, module.description, ref slider2);

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineHorizontal(rect.x + 45, rect.y + 30, rect.width - 80);
            GUI.color = Color.white;

            Color bColor = canUse.ContainsKey(module) ? Color.gray : Color.white;
            Rect buttonRect = new Rect(rect.x + 15, rect.y + 315, rect.width - 20, 20);

            Text.Anchor = TextAnchor.MiddleCenter;
            if (DrawCustomButton(buttonRect, "Station_InstallModule".Translate(), bColor))
            {
                if(canUse.ContainsKey(module))
                {
                    Messages.Message("Station_ModuleCannotInstal".Translate(canUse[module]), MessageTypeDefOf.NeutralEvent, false);
                }
                else
                {
                   CarryModule(module);
                }
            }
            if (canUse.ContainsKey(module))
            {
                Widgets.Label(new Rect(rect.x + 15, buttonRect.y - 2, rect.width - 20, 60), canUse[module]);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void CarryModule(ArmorModuleDef def)
        {
            Thing item = FindModuleItem(def.Item);

            if (item != null)
            {
                mkStationWindow.Close();
                Close();

                Job job = new Job(RimArmorCore.JobDefOfLocal.SetupModuleForArmor, mkStationWindow.mkStation, item, mkStationWindow.mkStation.ContainedArmor);
                job.count = 1;
                mkStationWindow.SelPawn.jobs.TryTakeOrderedJob(job);
            }
        }

        private Thing FindModuleItem(ThingDef itemDef)
        {
            Map map = mkStationWindow.mkStation.Map;
            List<SlotGroup> allGroupsListForReading = map.haulDestinationManager.AllGroupsListForReading;
            for (int i = 0; i < allGroupsListForReading.Count; i++)
            {
                SlotGroup slotGroup = allGroupsListForReading[i];
                foreach (var item in slotGroup.HeldThings)
                {
                    if (item.def == itemDef)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private bool DrawCustomButton(Rect rect, string label, Color textColor)
        {
            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(rect);
            GUI.color = CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(rect);
            return Widgets.ButtonInvisible(rect);
        }
    }
}
