using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents
{
    public class ITab_ModifyMk1 : ITab
    {
        public Mk1PowerStation Station => SelThing as Mk1PowerStation;
        public Apparel_MkArmor Mk1 => Station.ContainedArmor as Apparel_MkArmor;

        protected override void FillTab()
        {
            Text.Font = GameFont.Small;

            if (!Station.HasArmor)
            {
                Widgets.Label(new Rect(20, 20, size.x, size.y), "NoArmorInStation".Translate());
                return;
            }

            Rect r = new Rect(10, 30, 210, 20);

            Widgets.Label(new Rect(r.x, r.y, 50, 20), "CoreType".Translate());
            if (Widgets.ButtonText(new Rect(65, r.y, 140, 20), Mk1.Core == null ? "NoCore".Translate() : Mk1.LabelCap))
            {
                List<Thing> cores = GetCores();

                if (cores.Count == 0)
                {
                    Messages.Message("NoAvaliableCores".Translate(), MessageTypeDefOf.NeutralEvent, false);
                    return;
                }


                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (var core in cores)
                {
                    list.Add(new FloatMenuOption($"{core.LabelCap}", delegate
                    {
                        List<FloatMenuOption> list1 = new List<FloatMenuOption>();
                        foreach (var p in Station.Map.mapPawns.FreeColonists)
                        {
                            list1.Add(new FloatMenuOption($"{p.Name}", delegate
                            {
                                Job job = new Job(RimArmorCore.JobDefOfLocal.CarryReactorToStation, Station, core);
                                job.count = 1;
                                p.jobs.TryTakeOrderedJob(job);
                            }));
                        }

                        Find.WindowStack.Add(new FloatMenu(list1));

                    }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }

            r.y += 25;

            if (!Station.HasPower)
            {
                Widgets.Label(r, "NoPowerRightNow".Translate());
                return;
            }

            if (Mk1.Core != null)
            {
                Widgets.Label(r, "EnergyChargeCapacity".Translate(Mk1.EnergyCharge.ToString("f2"), Mk1.CoreComp.PowerCapacity));
                r.y += 25;
                Widgets.Label(r, "ChargeSpeed".Translate(Station.ChargeSpeed));
                r.y += 25;
                if (!Mk1.FullCharge)
                {
                    Widgets.Label(r, "EnabledCharging".Translate());
                }
                else
                {
                    Widgets.Label(r, "FullCharge".Translate());
                }
            }
        }

        private List<Thing> GetCores()
        {
            List<Thing> chosenThings = new List<Thing>();

            List<SlotGroup> allGroupsListForReading = Station.Map.haulDestinationManager.AllGroupsListForReading;
            for (int i = 0; i < allGroupsListForReading.Count; i++)
            {
                SlotGroup slotGroup = allGroupsListForReading[i];
                foreach (var item in slotGroup.HeldThings)
                {
                    if (!chosenThings.Contains(item) && item.TryGetComp<MoreEvents.Things.Mk1.ArmorCore>() != null)
                    {
                        chosenThings.Add(item);
                    }
                }
            }

            return chosenThings;
        }

        public ITab_ModifyMk1()
        {
            labelKey = "Station".Translate();
            size = new Vector2(230, 400);
        }
    }
}
