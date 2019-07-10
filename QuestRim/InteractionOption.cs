using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public abstract class InteractionOption : IExposable
    {
        public virtual int SortOrder { get; set; }

        public abstract string Label { get; }

        public virtual Color TextColor => Color.white;

        public bool Enabled = true;

        public abstract void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref Enabled, "Enabled");
        }
    }
}
