using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public abstract class CommOption : IExposable
    {
        public abstract string Label { get; }

        public virtual string Description { get; set; } = string.Empty;

        public virtual Color TextColor => Color.white;

        public abstract void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {

        }
    }
}
