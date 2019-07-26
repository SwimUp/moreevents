using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.Events.ConcantrationCamp
{
    public class ConcantrationCamp : VisitableSite
    {
        public override bool CanLeave()
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(this.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(Faction, new IntVec3(56, 0, 55));
            Lord lord = LordMaker.MakeNewLord(Faction, lordJob_DefendBase, Map);

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.ConcantrationCamp, Map, out List<Pawn> pawns, true, true, true, false, true, true, true, Faction, lord);

            GeneratePrisoners();
        }

        private void GeneratePrisoners()
        {
            Room room = GridsUtility.GetRoom(new IntVec3(65, 0, 80), Map);
        }
    }
}
