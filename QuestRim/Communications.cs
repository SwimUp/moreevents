using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class Communications : IExposable
    {
        public List<CommunicationDialog> CommunicationDialogs;

        //public List<Quest> Quests;

        public Communications()
        {
            CommunicationDialogs = new List<CommunicationDialog>();
        }

        public void OpenCommunications(Pawn speaker)
        {
            Find.WindowStack.Add(new GeoscapeWindow(this, speaker));
        }

        public void AddCommunication(CommunicationDialogDef def, Faction faction = null)
        {
            CommunicationDialog comDialog = new CommunicationDialog
            {
                CommDef = def,
                Faction = faction
            };

            CommunicationDialogs.Add(comDialog);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref CommunicationDialogs, "CommunicationDialogs", LookMode.Deep);
        }
    }
}
