using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim.Wars
{
    public class WarGoalWorker_Destruction : WarGoalWorker
    {
        private int lastObjectsCount;

        public int WinTrust => 15;

        private WorldObjectsHolder objects => Find.WorldObjects;

        public override void Tick()
        {
            base.Tick();

            if (Find.TickManager.TicksGame % 10000 == 0)
            {
                if (lastObjectsCount != objects.WorldObjectsCount)
                {
                    CheckState();
                }
            }
        }

        private void CheckState()
        {
            lastObjectsCount = objects.WorldObjectsCount;

            if(!objects.MapParents.Any(x => war.AssaultFactions.Contains(x.Faction)))
            {
                war.EndWar(Winner.Defenders);
                return;
            }
            if (!objects.MapParents.Any(x => war.DefendingFactions.Contains(x.Faction)))
            {
                war.EndWar(Winner.Assaulters);
                return;
            }
        }

        public override void EndWar(Winner winner)
        {
            base.EndWar(winner);

            if (WarUtility.WarWithPlayer(war))
            {
                var playerInteraction = QuestsManager.Communications.FactionManager.PlayerInteraction;
                if ((winner == Winner.Assaulters && war.DeclaredWarFaction == playerInteraction) || (winner == Winner.Defenders && war.DefendingFaction == playerInteraction))
                {
                    Find.LetterStack.ReceiveLetter("WarGoalWorker_Destruction_YouWinTitle".Translate(), "WarGoalWorker_Destruction_YouWinDesc".Translate(war.WarName), LetterDefOf.PositiveEvent);

                    var playerAlliance = QuestsManager.Communications.FactionManager.PlayerAlliance;
                    if (playerAlliance != null)
                    {
                        playerAlliance.GiveTrustToAllFactions(WinTrust);
                    }
                }
                else
                {
                    Find.LetterStack.ReceiveLetter("WarGoalWorker_Destruction_YouLoseTitle".Translate(), "WarGoalWorker_Destruction_YouLoseDesc".Translate(war.WarName), LetterDefOf.NegativeEvent);
                }
            }
        }

        public override void StartWar(War war)
        {
            base.StartWar(war);

            war.AttackedAlliance.GiveTrustToAllFactions(-15);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastObjectsCount, "lastObjectsCount");
        }
    }
}
