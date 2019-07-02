using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class DoomsdayUltimatumComp : WorldObjectComp
    {
        public int Timer;

        public DoomsdaySite Parent;

        private bool end = false;

        public int PlayerSilver
        {
            get
            {
                int count = 0;
                Find.Maps.ForEach(map =>
                {
                    if(map.IsPlayerHome)
                    {
                        count += map.resourceCounter.Silver;
                    }
                });

                return count;
            }
        }
        public int FactionSilver = 0;

        public int TotalSilverInBase => PlayerSilver + FactionSilver;

        public List<Faction> HelpingFactions = new List<Faction>();
        public readonly int MaxFactions = 3;
        public Dictionary<Faction, List<FactionRelation>> CachedRelations;
        public bool SupportFormed => HelpingFactions.Count == MaxFactions;
        private string helpFactionList;

        public DoomsdayUltimatumComp()
        {
            Parent = (DoomsdaySite)parent;
        }

        public void SetTimer(int days) => Timer = days * 60000;

        public void AddFaction(Faction f)
        {
            if (HelpingFactions.Count == MaxFactions)
                return;

            HelpingFactions.Add(f);

            if(HelpingFactions.Count == 1)
            {
                helpFactionList += $"{f.Name}";
            }
            else
            {
                helpFactionList += $",{f.Name}";
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            Timer--;

            if (!end && Timer <= 0)
            {
                end = true;
                EndGameDialogMessage("PlanetDestroyed".Translate(), Color.white);
                return;
            }

        }

        public void EndGameDialogMessage(string msg, Color screenFillColor)
        {
            DiaNode diaNode = new DiaNode(msg);
            DiaOption diaOption2 = new DiaOption("END");
            diaOption2.action = delegate
            {
                GenScene.GoToMainMenu();
            };
            diaOption2.resolveTree = true;
            diaNode.options.Add(diaOption2);
            Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode, delayInteractivity: true);
            dialog_NodeTree.screenFillColor = screenFillColor;
            dialog_NodeTree.silenceAmbientSound = false;
            dialog_NodeTree.closeOnAccept = false;
            dialog_NodeTree.closeOnCancel = false;
            Find.WindowStack.Add(dialog_NodeTree);
            Find.Archive.Add(new ArchivedDialog(diaNode.text));
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref Timer, "Timer");
            Scribe_Values.Look(ref helpFactionList, "helpFactionList");
            Scribe_Values.Look(ref FactionSilver, "FactionSilver");
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Translator.Translate("PlanetWillBeDestroyed")}{GenDate.TicksToDays(Timer).ToString("f2")}");
            builder.Append($"\n{"PlayerSilverCount".Translate(PlayerSilver)}");
            builder.Append($"\n{"FactionGift".Translate(FactionSilver)}");

            builder.Append($"HelpingFactionsList".Translate());
            if(helpFactionList.Length > 0)
                builder.Append(helpFactionList);

            return builder.ToString();
        }
    }
}
