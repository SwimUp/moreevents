using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public abstract class QuestOption : IExposable
    {
        public abstract string Label { get; }

        public bool Enable = true;

        public virtual Color TextColor => Color.white;

        public abstract void DoAction(Quest quest, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref Enable, "Enable");
        }
    }
}
