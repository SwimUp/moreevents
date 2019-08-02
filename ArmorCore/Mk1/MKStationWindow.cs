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

        private static readonly Texture2D PowerLimitBarTexture;
        private static readonly Texture2D OverDriveBarTexture;
        private static readonly Texture2D PowerLimitBarBorderTexture;
        private static readonly Texture2D PowerLimitBarOverDriveBorderTexture;
        private static readonly Texture2D EmptyBarTex;

        private static readonly Texture2D EmptyBarTex2;
        private static readonly Texture2D EnergyBarFrame;
        private static readonly Texture2D EnergyBar;

        private static readonly Texture2D EnergyBarFrame_Rec;
        private static readonly Texture2D EnergyBar_Rec;

        private static readonly Texture2D HeadPart;
        private static readonly Texture2D BodyPart;

        private float bankChargeSpeed = 0f;

        private Vector2 headCategory = Vector2.zero;
        private Vector2 bodyCategory = Vector2.zero;
        private Vector2 legsCategory = Vector2.zero;
        private Vector2 mainCategory = Vector2.zero;

        private Vector2 empty = Vector2.zero;

        private Vector2 fullInfo = Vector2.zero;

        static MKStationWindow()
        {
            PowerLimitBarTexture = ContentFinder<Texture2D>.Get("PowerLimitBar");
            OverDriveBarTexture = ContentFinder<Texture2D>.Get("OverDriveBar");
            PowerLimitBarBorderTexture = ContentFinder<Texture2D>.Get("PowerLimitBarBorder");
            PowerLimitBarOverDriveBorderTexture = ContentFinder<Texture2D>.Get("PowerLimitBarOverDriveBorder");
            EmptyBarTex = ContentFinder<Texture2D>.Get("StationBack");
            EmptyBarTex2 = ContentFinder<Texture2D>.Get("StationBack2");
            EnergyBarFrame = ContentFinder<Texture2D>.Get("batteryframe");
            EnergyBar = ContentFinder<Texture2D>.Get("batterybar");

            HeadPart = ContentFinder<Texture2D>.Get("StationPart/Part_Head");
            BodyPart = ContentFinder<Texture2D>.Get("Things/Apparels/Mk1/Mk1");

            EnergyBarFrame_Rec = ContentFinder<Texture2D>.Get("batteryreactorframe");
            EnergyBar_Rec = ContentFinder<Texture2D>.Get("batteryreactorbar");
        }

        private enum Tab
        {
            Armor,
            Station
        }

        private Tab tab;

        public MKStationWindow()
        {
            doCloseX = true;
            forcePause = true;
        }

        public MKStationWindow(Mk1PowerStation station, Pawn pawn)
        {
            doCloseX = true;
            mkStation = station;
            SelPawn = pawn;
            forcePause = true;

            foreach (var slot in mkStation.Slots)
            {
                if(slot.Module != null)
                {
                    bankChargeSpeed += slot.Module.def.EnergyBankCharge;
                }
            }
        }

        private ref Vector2 GetVector(ArmorModuleCategory category)
        {
            switch (category)
            {
                case ArmorModuleCategory.Body:
                    return ref bodyCategory;
                case ArmorModuleCategory.General:
                    return ref mainCategory;
                case ArmorModuleCategory.Head:
                    return ref headCategory;
                case ArmorModuleCategory.Legs:
                    return ref legsCategory;
            }

            return ref empty;
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
                    DrawArmor(rect2);
                    break;
            }
        }

        private void DrawArmor(Rect rect)
        {
            if(mkStation.ContainedArmor == null)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, "StationInspectorInfo_NOARMOR".Translate());
                Text.Anchor = TextAnchor.UpperLeft;

                return;
            }

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineVertical(280, rect.y, rect.height);
            Widgets.DrawLineVertical(870, rect.y, rect.height); //110
            Widgets.DrawLineHorizontal(0, 360, 280);
            Widgets.DrawLineHorizontal(0, rect.y, rect.width);
            GUI.color = Color.white;

            Rect leftRect = new Rect(rect.x + 10, rect.y + 10, 1100, 680);
            Rect head = new Rect(leftRect.x + 32, leftRect.y - 10, 192, 192);
            Rect body = new Rect(leftRect.x, leftRect.y + 55, 256, 256);

            GUI.DrawTexture(body, BodyPart);
            GUI.DrawTexture(head, HeadPart);

            Rect coreInfoRect = new Rect(leftRect.x, 370, 270, 310);
            DrawCore(coreInfoRect);

            Rect moduleRect = new Rect(300, rect.y, 130, 630);
            foreach(var slot in mkStation.ContainedArmor.Slots)
            {
                DrawSlot(moduleRect, slot);
                moduleRect.x += 140;
            }

            Rect fullInfoRect = new Rect(875, rect.y + 5, 220, rect.height - 10);
            StringBuilder builder = new StringBuilder();
            foreach(var armorSlot in mkStation.ContainedArmor.Slots)
            {
                foreach(var slot in armorSlot.Modules)
                {
                    if(slot.Module != null)
                    {
                        builder.AppendLine(slot.Module.StatDescription());
                    }
                }
            }
            Widgets.LabelScrollable(fullInfoRect, $"{"Station_FullInfo".Translate(builder.ToString())}", ref fullInfo);
        }

        private void DrawSlot(Rect rect, ArmorSlot armorSlot)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x, rect.y + 7, 100, 20), armorSlot.Category.GetLabel());
            Text.Anchor = TextAnchor.UpperLeft;

            rect.y += 30;

            Rect rect2 = new Rect(0,0, 110, 110);
            Rect scrollVertRectFact = new Rect(0, 0, 110, armorSlot.Modules.Count * 120);
            Rect sliderRect = new Rect(rect.x, rect.y, rect.width, rect.height - 40);
            Widgets.BeginScrollView(sliderRect, ref GetVector(armorSlot.Category), scrollVertRectFact, false);
            foreach(var slot in armorSlot.Modules)
            {
                DrawSlotInfo(rect2, slot, armorSlot);
                rect2.y += 120;
            }
            Widgets.EndScrollView();
        }

        private void DrawSlotInfo(Rect rect, ModuleSlot<MKArmorModule> slot, ArmorSlot armorSlot)
        {
            Widgets.DrawAtlas(rect, Widgets.ButtonSubtleAtlas);
            if (slot.Module != null)
            {
                GUI.DrawTexture(new Rect(rect.x + 5, rect.y + 5, 100, 100), slot.Module.def.IconImage);

                string fullDesc = $"{slot.Module.def.LabelCap}\n\n{slot.Module.def.description}\n{slot.Module.StatDescription()}";
                TooltipHandler.TipRegion(rect, new TipSignal(fullDesc, 4321211));

                if(Widgets.ButtonInvisible(rect))
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
                                mkStation.ContainedArmor.Notify_ModulesChanges();
                            }
                            else
                            {
                                Messages.Message("NotEnoughSpace".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            }
                        })
                    }));
                }
            }
            else
            {
                if (Widgets.ButtonInvisible(rect))
                {
                    Find.WindowStack.Add(new ArmorModuleWindow(this, armorSlot.Category));
                }
            }

            Widgets.DrawHighlightIfMouseover(rect);
        }

        private void DrawCore(Rect rect)
        {
            Rect label = rect;
            label.height = 25;

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineHorizontal(70, label.y + 24, 140);
            Widgets.DrawLineHorizontal(60, label.y + 120, 160);
            Widgets.DrawLineHorizontal(0, label.y + 200, 280);
            GUI.color = Color.white;

            Rect r2 = new Rect(105, rect.y + 30, 70, 70);
            Widgets.DrawAtlas(r2, Widgets.ButtonSubtleAtlas);
            Widgets.DrawHighlightIfMouseover(r2);

            Text.Anchor = TextAnchor.MiddleCenter;
            if (DrawCustomButton(new Rect(10, rect.y + 210, 260, 30), "Station_InstallCore".Translate(), mkStation.ContainedArmor.Core == null ? Color.white : Color.grey))
            {
                if (mkStation.ContainedArmor.Core != null)
                    return;

                List<Thing> cores = mkStation.GetCores();
                if (cores.Count == 0)
                {
                    Messages.Message("NoAvaliableCores".Translate(), MessageTypeDefOf.NeutralEvent, false);
                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }

                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (var core in cores)
                {
                    list.Add(new FloatMenuOption($"{core.LabelCap}", delegate
                    {
                        Job job = new Job(JobDefOfLocal.CarryReactorToStation, mkStation, core);
                        job.count = 1;
                        SelPawn.jobs.TryTakeOrderedJob(job);

                        Close();
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }
            if (DrawCustomButton(new Rect(10, rect.y + 250, 260, 30), "Station_RemoveCore".Translate(), mkStation.ContainedArmor.Core == null ? Color.grey : Color.white))
            {
                if (mkStation.ContainedArmor.Core == null)
                    return;

                if (CellFinder.TryFindRandomCellNear(mkStation.Position, mkStation.Map, 3, null, out IntVec3 result))
                {
                    GenSpawn.Spawn(mkStation.ContainedArmor.Core, result, mkStation.Map);
                    mkStation.ContainedArmor.Core = null;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;

            if (mkStation.ContainedArmor.Core == null)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(label, "Station_NoCore".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(label, mkStation.ContainedArmor.Core.LabelCap);
                Widgets.Label(new Rect(rect.x, rect.y + 98, rect.width, 25), "Station_CoreParam".Translate());
                Text.Anchor = TextAnchor.UpperLeft;

                Widgets.Label(new Rect(rect.x, rect.y + 125, rect.width, 70), "Station_CoreInfo".Translate(mkStation.ContainedArmor.CoreComp.PowerCapacity));

                GUI.DrawTexture(new Rect(108, rect.y + 30, 64, 64), ContentFinder<Texture2D>.Get(mkStation.ContainedArmor.Core.def.graphicData.texPath));
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
            DrawCustomFillableBar(new Rect(infoRect.x + 210, infoRect.y, infoRect.width - 210, infoRect.height), mkStation.OverDrive, OverDriveBarTexture, EmptyBarTex, $"({mkStation.OverDrive.ToString("f2")} / 100)", Color.red, PowerLimitBarOverDriveBorderTexture);
            Text.Anchor = TextAnchor.UpperLeft;

            Rect secondRect = new Rect(560, rect.y, 530, rect.height);

            Rect labelRect = new Rect(560, secondRect.y + 10, 530, 200);
            if(mkStation.ContainedArmor == null || mkStation.ContainedArmor.Core == null)
                Widgets.Label(labelRect, "Station_SecondTableInfo".Translate(mkStation.Slots.Where(s => s.Module != null).Count(), mkStation.Slots.Count, mkStation.ChargeSpeed, 0, 0, mkStation.EnergyBankCharge.ToString("f2"), mkStation.EnergyBank, bankChargeSpeed.ToString("f2")));
            else
                Widgets.Label(labelRect, "Station_SecondTableInfo".Translate(mkStation.Slots.Where(s => s.Module != null).Count(), mkStation.Slots.Count, mkStation.ChargeSpeed, mkStation.ContainedArmor.EnergyCharge.ToString("f2"), mkStation.ContainedArmor.CoreComp.PowerCapacity, mkStation.EnergyBankCharge.ToString("f2"), mkStation.EnergyBank, bankChargeSpeed.ToString("f2")));

            DrawCustomFillableBarForEnergyBar(new Rect(560, 135, 200, 450), mkStation.EnergyBankCharge, mkStation.EnergyBank, EnergyBarFrame, EnergyBar, EmptyBarTex2);

            float armorCharge = 0f;
            float maxRef = 0f;
            if (mkStation.ContainedArmor.CoreComp != null)
            {
                armorCharge = mkStation.ContainedArmor.EnergyCharge;
                maxRef = mkStation.ContainedArmor.CoreComp.PowerCapacity;
            }
            DrawCustomFillableBarForEnergyBar(new Rect(840, 135, 200, 450), armorCharge, maxRef, EnergyBarFrame_Rec, EnergyBar_Rec, EmptyBarTex2);
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

        private void DrawCustomFillableBarForEnergyBar(Rect mainRect, float fill, float maxRef, Texture2D border, Texture2D fillTexture, Texture2D emptyTexture)
        {
            GUI.DrawTexture(mainRect, border);
            GUI.DrawTexture(mainRect, fillTexture);

            maxRef = Mathf.Clamp(maxRef, 1, maxRef);
            float offset = mainRect.height * (fill / maxRef);
            float maxHeight = Mathf.Clamp(mainRect.height - offset, 0, mainRect.height);
            GUI.DrawTexture(new Rect(mainRect.x, mainRect.y, mainRect.width, maxHeight), emptyTexture);
        }

        private void DrawModulesSlots(Rect rect)
        {
            foreach(var slot in mkStation.Slots.OrderBy(x => x.Order))
            {
                DrawModuleSlot(rect, slot);
                rect.y += 112;
            }
        }

        private void DrawModuleSlot(Rect rect, ModuleSlot<MKStationModule> slot)
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
