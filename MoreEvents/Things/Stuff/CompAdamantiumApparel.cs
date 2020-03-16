using MoreEvents;
using RimOverhaul.Hediffs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompAdamantiumApparel : ThingComp
    {
        public override void Notify_SignalReceived(Signal signal)
        {
            if (signal.tag == "apparel-wear")
            {
                if (signal.args.TryGetArg(0, out NamedArgument arg))
                {
                    Pawn pawn = arg.arg as Pawn;
                    if (pawn != null)
                    {
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOfLocal.AdamantiumToxin);
                        if (hediff != null)
                        {
                            HediffComp_SeverityPerDayManaged comp = hediff.TryGetComp<HediffComp_SeverityPerDayManaged>();
                            if (comp != null)
                            {
                                comp.AddSeverity = true;
                            }
                        }
                        else
                        {
                            hediff = HediffMaker.MakeHediff(HediffDefOfLocal.AdamantiumToxin, pawn);
                            hediff.Severity = 0.1f;
                            pawn.health.AddHediff(hediff);
                        }
                    }
                }
            }
            if (signal.tag == "apparel-unwear")
            {
                if (signal.args.TryGetArg(0, out NamedArgument arg))
                {
                    Pawn pawn = arg.arg as Pawn;
                    if (pawn != null)
                    {
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOfLocal.AdamantiumToxin);
                        if (hediff != null)
                        {
                            HediffComp_SeverityPerDayManaged comp = hediff.TryGetComp<HediffComp_SeverityPerDayManaged>();
                            if (comp != null)
                            {
                                comp.AddSeverity = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
