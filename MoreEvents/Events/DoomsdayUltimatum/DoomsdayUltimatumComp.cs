using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
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

        public DoomsdayUltimatumComp()
        {
            Parent = (DoomsdaySite)parent;
        }

        public void SetTimer(int days) => Timer = days * 60000;

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
            Scribe_Values.Look(ref FactionSilver, "FactionSilver");
        }

        public override string CompInspectStringExtra()
        {
            string result = $"{Translator.Translate("PlanetWillBeDestroyed")}{GenDate.TicksToDays(Timer).ToString("f2")}";
            result += $"\n{"PlayerSilverCount".Translate(PlayerSilver)}";
            result += $"\n{"FactionGift".Translate(FactionSilver)}";

            return result;
        }
    }
}
