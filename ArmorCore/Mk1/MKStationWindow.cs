using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1
{
    [StaticConstructorOnStartup]
    public class MKStationWindow : Window
    {
        public Mk1PowerStation mkStation;

        private Vector2 scroll1 = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(1100, 700);
        private static List<TabRecord> tabsList = new List<TabRecord>();

        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
        private static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;

        private enum Tab
        {
            Armor,
            Station
        }

        private Tab tab;

        public MKStationWindow()
        {

        }

        public MKStationWindow(Mk1PowerStation station)
        {
            mkStation = station;
            forcePause = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord("StationTab".Translate(), delegate
            {
                tab = Tab.Station;
            }, tab == Tab.Station));
            tabsList.Add(new TabRecord("ArmorTab".Translate(), delegate
            {
                tab = Tab.Armor;
            }, tab == Tab.Armor));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 550);
            tabsList.Clear();

            switch (tab)
            {
                case Tab.Station:
                    DrawStation(rect2);
                    break;
                case Tab.Armor:

                    break;
            }
        }

        private void DrawStation(Rect rect)
        {
            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineVertical(536, rect.y, 550);
            Widgets.DrawLineHorizontal(0, rect.y + 550, 1100);
            GUI.color = Color.white;

            Rect modulesRect = new Rect(rect.x + 10, rect.y + 10, 515, 540);
            DrawModulesSlots(modulesRect);
        }

        private void DrawModulesSlots(Rect rect)
        {
            foreach(var slot in mkStation.Slots.OrderBy(x => x.Order))
            {
                DrawModuleSlot(rect, slot);
                rect.y += 112;
            }
        }

        private void DrawModuleSlot(Rect rect, ModuleSlot slot)
        {
            Rect moduleRect = new Rect(rect.x, rect.y, 515, 102);

            if(slot.Module == null)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                if(DrawCustomButton(moduleRect, "Station_NoModule".Translate(), Color.white))
                {
                    Find.WindowStack.Add(new ModuleMenuWindow(this));
                }
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
              //  Widgets.DrawBoxSolid(moduleRect, SlotBGColor);

                GUI.color = MenuSectionBGBorderColor;
                Widgets.DrawLineHorizontal(moduleRect.x + 142, moduleRect.y + 21, 328);
                GUI.color = Color.white;

                Rect textureRect = new Rect(rect.x + 3, rect.y + 3, 90, 90);
                GUI.DrawTexture(textureRect, slot.Module.def.IconImage);

                Rect titleRect = new Rect(rect.x + 104, rect.y + 1, 412, 25);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(titleRect, slot.Module.def.LabelCap);
                Rect mainRect = new Rect(titleRect.x, titleRect.y + 22, 410, 75);
                Text.Anchor = TextAnchor.UpperLeft;
                string desc = slot.Module.StatDescription();
                Widgets.LabelScrollable(mainRect, desc, ref scroll1);

                TooltipHandler.TipRegion(moduleRect, new TipSignal(desc, 012312312));
            }

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(moduleRect);
            GUI.color = Color.white;
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
