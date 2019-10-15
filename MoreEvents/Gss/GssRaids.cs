using DarkNET.CommunicationComps;
using MoreEvents;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gss
{
    public static class GssRaids
    {
        public static Dictionary<PawnKindDef, float> PawnKinds = new Dictionary<PawnKindDef, float>
        {
            { PawnKindDefOfLocal.Pawn_EliteGrenadier, 8 },
            { PawnKindDefOfLocal.Pawn_EliteShotgun, 10 },
            { PawnKindDefOfLocal.Pawn_FrontGrenadier, 12 },
            { PawnKindDefOfLocal.Pawn_HeavyShooter, 45 },
            { PawnKindDefOfLocal.Pawn_Infantryman, 20 },
            { PawnKindDefOfLocal.Pawn_Infantryman_Elite, 15 },
            { PawnKindDefOfLocal.Pawn_LightShooter, 65 },
        };

        public static Faction GssFaction => Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.GalacticSecurityService);

        public static void SendRaid(Map map, float points)
        {
            float totalPower = points;
            List<PawnKindDef> selectedKinds = new List<PawnKindDef>();

            while (totalPower > 0)
            {
                PawnKindDef selected = PawnKinds.RandomElementByWeight(x => x.Value).Key;

                selectedKinds.Add(selected);
                totalPower -= selected.combatPower;
            }

            if (selectedKinds.Count == 0)
                return;

            List<Pawn> outPawns = new List<Pawn>();
            foreach (PawnKindDef kind in selectedKinds)
            {
                PawnGenerationRequest request = new PawnGenerationRequest(kind, GssFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, true, false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, null);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                outPawns.Add(pawn);

                Find.WorldPawns.PassToWorld(pawn);
            }

            if (outPawns.Count == 0)
                return;

            CommunicationComponent_GssRaidTimer timer = new CommunicationComponent_GssRaidTimer((int)(Rand.Range(0.3f, 1.5f) * 60000), outPawns, map);
            timer.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();

            QuestsManager.Communications.RegisterComponent(timer);

            Find.LetterStack.ReceiveLetter("DarkNet_RaidSendedTitle".Translate(), "DarkNet_RaidSendedDesc".Translate(GssFaction.Name), LetterDefOf.ThreatBig);
        }

        public static PawnsArrivalModeDef ResolveRaidArriveMode(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            if(Rand.Chance(0.35f))
            {
                DropCellFinder.TryFindRaidDropCenterClose(out parms.spawnCenter, map);
                return PawnsArrivalModeDefOf.CenterDrop;
            }

            CellFinder.TryFindRandomEdgeCellWith(x => x.Walkable(map) && !x.Fogged(map) && x.Standable(map), map, 0f, out parms.spawnCenter);
            return PawnsArrivalModeDefOf.EdgeWalkIn;
        }

        public static RaidStrategyDef ResolveRaidStrategy()
        {
            return RaidStrategyDefOf.ImmediateAttack;
        }
    }
}
