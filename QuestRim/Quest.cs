using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public enum EndCondition
    {
        Timeout,
        Fail,
        Success,
        None
    };

    public abstract class Quest : IExposable
    {
        public string UniqueId;

        public abstract string CardLabel { get; }
        public abstract string Description { get; }

        public Faction Faction;
        public LookTargets Target;

        public List<Thing> Rewards = new List<Thing>();
        public List<QuestOption> Options;

        public int TicksToPass = 60000;
        public bool UnlimitedTime = false;

        public virtual void Tick()
        {
            if (!UnlimitedTime)
            {
                TicksToPass--;
                if (TicksToPass <= 0)
                {
                    EndQuest(EndCondition.Timeout);
                }
            }
        }

        public virtual void EndQuest(EndCondition condition = EndCondition.None)
        {
            for (int i = 0; i < Rewards.Count; i++)
            {
                Rewards[i].Destroy();
            }

            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public virtual void DrawAdditionalOptions(Rect rect)
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref UniqueId, "UniqueId");
            Scribe_Values.Look(ref TicksToPass, "TicksToPass");
            Scribe_Values.Look(ref UnlimitedTime, "UnlimitedTime");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Deep.Look(ref Target, "Target");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
            Scribe_Collections.Look(ref Rewards, "Rewards", LookMode.Deep);
        }
    }
}
