using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events.Competitions
{
    public class CommOption_CompetitionsPaid : CommOption
    {
        public override string Label => throw new NotImplementedException();

        public WorldObject_Competitions WorldObject_Competitions;

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if(WorldObject_Competitions != null)
            {

            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref WorldObject_Competitions, "WorldObject_Competitions");
        }
    }
}
