using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim.Actions
{
    public class CommAction_OpenDialogWithFaction : CommAction
    {
        public DialogDef Dialog;

        public CommAction_OpenDialogWithFaction(DialogDef def) => Dialog = def;

        public CommAction_OpenDialogWithFaction() { }

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if(speaker != null)
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (Faction item in Find.FactionManager.AllFactionsVisibleInViewOrder)
                {
                    if (item.leader != null)
                    {
                        FactionRelationKind rel = item.RelationKindWith(speaker.Faction);
                        list.Add(new FloatMenuOption($"{item.Name} - {rel}", delegate
                        {
                            Dialog dia = new Dialog(Dialog, speaker, item.leader);
                            dia.Init();
                            Find.WindowStack.Add(dia);
                        }));
                    }
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref Dialog, "Dialog");
        }
    }
}
