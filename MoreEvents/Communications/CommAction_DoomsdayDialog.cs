
using DiaRim;
using MoreEvents.Events.DoomsdayUltimatum;
using QuestRim;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreEvents.Communications
{
    public enum Answer
    {
        Yes,
        No,
        None
    }

    public class CommAction_DoomsdayDialog : CommAction
    {

        private DialogDef Dialog => DialogDefOfLocal.DoomsdayHelp;

        private List<Faction> factions = new List<Faction>();
        private List<Answer> answers = new List<Answer>();
        private DoomsdaySite site;

        private Faction currentFaction;

        public CommAction_DoomsdayDialog()
        {
        }

        public CommAction_DoomsdayDialog(DoomsdaySite site, Faction attacker)
        {
            this.site = site;

            foreach(var faction in Find.FactionManager.AllFactionsVisibleInViewOrder.Where(x => !x.IsPlayer && x != attacker && x.RelationKindWith(site.Faction) == FactionRelationKind.Hostile))
            {
                factions.Add(faction);
                answers.Add(Answer.None);
            }
        }

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if (speaker != null)
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                if (site.comp.SupportFormed)
                {
                    list.Add(new FloatMenuOption("SupportFullDoomsday".Translate(), delegate
                    {
                        Messages.Message("SupportFullDoomsday".Translate(), MessageTypeDefOf.NeutralEvent, false);
                    }));
                }
                else
                {
                    foreach (var faction in factions)
                    {
                        if (faction.leader != null)
                        {
                            FactionRelationKind rel = faction.RelationKindWith(speaker.Faction);
                            Answer answer = GetAnswer(faction);
                            list.Add(new FloatMenuOption($"{faction.Name} - {rel} [{answer.TranslateAnswer()}]", delegate
                            {
                                if (answer == Answer.None)
                                {
                                    currentFaction = faction;

                                    Dialog dia = new Dialog(Dialog, speaker, faction.leader);
                                    dia.Init();
                                    dia.CloseAction = CheckAnswer;
                                    Find.WindowStack.Add(dia);
                                }
                            }));
                        }
                    }
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private void CheckAnswer(string answer)
        {
            if(answer == "удача")
            {
                SetAnswer(currentFaction, Answer.Yes);
                site.comp.AddFaction(currentFaction);
            }
            if(answer == "неудача")
            {
                SetAnswer(currentFaction, Answer.No);
            }
        }

        private Answer GetAnswer(Faction faction)
        {
            int i;
            for (i = 0; i < factions.Count; i++)
            {
                Faction f = factions[i];
                if (f == faction)
                    return answers[i];
            }

            return Answer.None;
        }

        private void SetAnswer(Faction faction, Answer answer)
        {
            int i;
            for (i = 0; i < factions.Count; i++)
            {
                Faction f = factions[i];
                if (f == faction)
                {
                    answers[i] = answer;
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref factions, "factions", LookMode.Reference);
            Scribe_Collections.Look(ref answers, "answers", LookMode.Value);
            Scribe_References.Look(ref site, "site");
        }
    }

    public static class AnswerExtension
    {
        public static string TranslateAnswer(this Answer answer)
        {
            switch(answer)
            {
                case Answer.No:
                    return "NoHelpDoomsday".Translate();
                case Answer.None:
                    return "UnknownDoomsdayStatus".Translate();
                case Answer.Yes:
                    return "HelpDoomsday".Translate();
            }

            return "";
        }
    }
}
