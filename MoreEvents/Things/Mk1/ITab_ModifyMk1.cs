using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents
{
    public class ITab_ModifyMk1 : ITab
    {
        public Mk1PowerStation Station => SelThing as Mk1PowerStation;
        public Apparel_Mk1 Mk1 => Station.ContainedArmor as Apparel_Mk1;

        protected override void FillTab()
        {
            Text.Font = GameFont.Small;

            if (!Station.HasArmor)
            {
                Widgets.Label(new Rect(20, 20, size.x, size.y), "NoArmorInStation".Translate());
                return;
            }

            Rect r = new Rect(10, 30, 180, 20);
            Widgets.Label(r, "EnergyChargeCapacity".Translate(Mk1.EnergyCharge.ToString("f2")));
            r.y += 25;
            Widgets.Label(r, "ChargeSpeed".Translate(Station.ChargeSpeed));
            r.y += 25;
            if (!Station.HasPower)
            {
                Widgets.Label(r, "NoPowerRightNow".Translate());
            }
            else if (!Mk1.FullCharge)
            {
                Widgets.Label(r, "EnabledCharging".Translate());
            }
            else
            {
                Widgets.Label(r, "FullCharge".Translate());
            }


            Widgets.Label(new Rect(110, size.y - 190, 50, 64), "CoreType".Translate());
            Widgets.DrawBox(new Rect(70, size.y - 180, 68, 68));
            Widgets.DrawAtlas(new Rect(70, size.y - 180, 64, 64), ContentFinder<Texture2D>.Get("Things/Buildings/ColdFusion/Core"));

            Widgets.Label(new Rect(size.x - 100, size.y - 110, 50, 64), "CoreType".Translate());
            Widgets.DrawBox(new Rect(size.x - 88, size.y - 90, 66, 66));
            Widgets.DrawAtlas(new Rect(size.x - 88, size.y - 90, 64, 64), ContentFinder<Texture2D>.Get("Things/Buildings/ColdFusion/Core"));

            Widgets.Label(new Rect(20, size.y - 120, 50, 64), "CoreType".Translate());
            Widgets.DrawBox(new Rect(20, size.y - 90, 66, 66));
            Widgets.DrawAtlas(new Rect(20, size.y - 90, 64, 64), ContentFinder<Texture2D>.Get("Things/Buildings/ColdFusion/Core"));
        }

        public ITab_ModifyMk1()
        {
            labelKey = "Station".Translate();
            size = new Vector2(200, 400);
        }
    }
}
