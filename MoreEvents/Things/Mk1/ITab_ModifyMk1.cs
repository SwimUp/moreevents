using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents
{
    public class ITab_ModifyMk1 : ITab
    {
        protected override void FillTab()
        {
            Log.Message("1");
        }

        public ITab_ModifyMk1()
        {
            labelKey = "Station".Translate();
        }
    }
}
