using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Events.Competitions
{
    public class CommOption_CompetitionsPaid : CommOption
    {
        public override string Label => "CommOption_CompetitionsPaid".Translate();

        public WorldObject_Competitions WorldObject_Competitions;

        private bool used;

        public override Color TextColor => used ? Color.yellow : Color.white;

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if (WorldObject_Competitions != null)
            {
                used = WorldObject_Competitions.Paid(speaker.Map);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref WorldObject_Competitions, "WorldObject_Competitions");
            Scribe_Values.Look(ref used, "used");
        }
    }
}
