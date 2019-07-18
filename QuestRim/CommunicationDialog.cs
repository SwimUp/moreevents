﻿using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class CommunicationDialog : IExposable, ILoadReferenceable
    {
        public int id;

        public string CardLabel;
        public string Description;

        public bool ShowInConsole = true;

        public IncidentDef RelatedIncident;
        public Faction Faction;
        public List<CommOption> Options;

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");

            Scribe_Values.Look(ref CardLabel, "CardLabel");
            Scribe_Values.Look(ref Description, "Description");

            Scribe_Defs.Look(ref RelatedIncident, "RelatedIncident");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);

            Scribe_Values.Look(ref ShowInConsole, "ShowInConsole");
        }

        public virtual void Destroy()
        {
            QuestsManager.Communications.RemoveCommunication(this);
        }

        public void OpenDialog(Pawn speaker, Pawn defendant)
        {
            Find.WindowStack.Add(new CommunicationDialogWindow(this, speaker, defendant));
        }

        public string GetUniqueLoadID()
        {
            return "ComDialog_" + id;
        }
    }
}
