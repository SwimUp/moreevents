using MoreEvents.Things.Mk1;
using RimWorld;
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
        public Pawn SelPawn;

        private Vector2 scroll1 = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(1100, 700);
        private static List<TabRecord> tabsList = new List<TabRecord>();

        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
        private static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;

        private static readonly Texture2D PowerLimitBarTexture = ContentFinder<Texture2D>.Get("PowerLimitBar");
        private static readonly Texture2D OverDriveBarTexture = ContentFinder<Texture2D>.Get("OverDriveBar");
        private static readonly Texture2D PowerLimitBarBorderTexture = ContentFinder<Texture2D>.Get("PowerLimitBarBorder");
        private static readonly Texture2D PowerLimitBarOverDriveBorderTexture = ContentFinder<Texture2D>.Get("PowerLimitBarOverDriveBorder");
        private static readonly Texture2D EmptyBarTex = ContentFinder<Texture2D>.Get("StationBack");

        private float bankChargeSpeed = 0f;

        private enum Tab
        {
            Armor,
            Station
        }

        private Tab tab;

        public MKStationWindow()
        {

        }

        public MKStationWindow(Mk1PowerStation station, Pawn pawn)
        {
            mkStation = station;
            SelPawn = pawn;

            foreach(var slot in mkStation.Slots)
            {
                if(slot.Module != null)
                {
                    bankChargeSpeed += slot.Module.def.EnergyBankCharge;
                }
            }
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
            Widgets.DrawLineHorizontal(0, 376, 536);
            GUI.color = Color.white;

            Rect modulesRect = new Rect(rect.x + 10, rect.y + 10, 515, 540);
            DrawModulesSlots(modulesRect);

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect infoRect = new Rect(rect.x + 10, 380, 515, 30); //180
            Widgets.Label(infoRect, mkStation.PowerLimit > 100f ? "Station_PowerLimit".Translate() : "Station_PowerLimitInfo".Translate());
            infoRect.y += 27;
            DrawCustomFillableBar(infoRect, mkStation.PowerLimit, PowerLimitBarTexture, EmptyBarTex, $"({mkStation.PowerLimit.ToString("f2")} / 100)", mkStation.PowerLimit > 100f ? Color.red : Color.black, PowerLimitBarBorderTexture);

            infoRect.y += 40;
            Rect bRect = new Rect(rect.x + 10, infoRect.y, 200, 30);
            Text.Anchor = TextAnchor.MiddleCenter;
            if (DrawCustomButton(bRect, "Station_OverDrive".Translate(), mkStation.CanOverDrive ? mkStation.OverDriveEnabled ? Color.red : Color.white : Color.grey))
            {
                mkStation.SetOverDrive(!mkStation.OverDriveEnabled);
            }
            DrawCustomFillableBar(new Rect(infoRect.x + 210, infoRect.y, infoRect.width - 210, infoRect.height), mkStation.OverDrive / 100, OverDriveBarTexture, EmptyBarTex, $"({mkStation.OverDrive.ToString("f2")} / 100)", Color.red, PowerLimitBarOverDriveBorderTexture);
            Text.Anchor = TextAnchor.UpperLeft;

            Rect secondRect = new Rect(560, rect.y, 530, rect.height);

            Rect labelRect = new Rect(560, secondRect.y + 10, 530, 200);
            Widgets.Label(labelRect, "Station_SecondTableInfo".Translate(mkStation.Slots.Where(s => s.Module != null).Count(), mkStation.Slots.Count, mkStation.ChargeSpeed, mkStation.EnergyBankCharge, mkStation.EnergyBank, bankChargeSpeed.ToString("f2")));
        }

        private void DrawCustomFillableBar(Rect mainRect, float fill, Texture2D fillTexture, Texture2D emptyTexture)
        {
            float offset = mainRect.width * (fill / 100);
            float maxWidth = Mathf.Clamp(mainRect.width - offset, 0, mainRect.width);
            GUI.DrawTexture(mainRect, fillTexture);
            GUI.DrawTexture(new Rect(mainRect.x + offset, mainRect.y, maxWidth, 30), emptyTexture);
        }

        private void DrawCustomFillableBar(Rect mainRect, float fill, Texture2D fillTexture, Texture2D emptyTexture, string text, Color textColor)
        {
            float offset = mainRect.width * (fill / 100);
            GUI.DrawTexture(mainRect, fillTexture);
            float maxWidth = Mathf.Clamp(mainRect.width - offset, 0, mainRect.width);
            GUI.DrawTexture(new Rect(mainRect.x + offset, mainRect.y, maxWidth, mainRect.height), emptyTexture);
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = textColor;
            Widgets.Label(mainRect, text);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawCustomFillableBar(Rect mainRect, float fill, Texture2D fillTexture, Texture2D emptyTexture, string text, Color textColor, Texture2D border)
        {
            float offset = mainRect.width * (fill / 100);
            GUI.DrawTexture(mainRect, fillTexture);
            float maxWidth = Mathf.Clamp(mainRect.width - offset, 0, mainRect.width);
            GUI.DrawTexture(new Rect(mainRect.x + offset, mainRect.y, maxWidth, mainRect.height), emptyTexture);
            GUI.DrawTexture(mainRect, border);
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = textColor;
            Widgets.Label(mainRect, text);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
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

                if(Widgets.ButtonInvisible(moduleRect))
                {
                    Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Station_UnloadModule".Translate(), delegate
                        {
                            if (CellFinder.TryFindRandomCellNear(mkStation.Position, mkStation.Map, 3, null, out IntVec3 result))
                            {
                                GenSpawn.Spawn(slot.Item, result, mkStation.Map);
                                slot.Item = null;
                                slot.Module = null;
                                mkStation.Notify_ModulesChanges();
                            }
                            else
                            {
                                Messages.Message("NotEnoughSpace".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            }
                        })
                    }));
                }
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
