using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class QuestsManager : GameComponent
    {
        public static Communications Communications
        {
            get
            {
                if(communications == null)
                {
                    communications = new Communications();
                }

                return communications;
            }
        }
        private static Communications communications;

        public QuestsManager()
        {
            communications = null;
            Building_Geoscape.PlayerHasGeoscape = false;
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (communications != null)
            {
                if(Find.TickManager.TicksGame % 500 == 0)
                {
                    for (int i = 0; i < communications.QuestPawns.Count; i++)
                    {
                        QuestPawn questPawn = communications.QuestPawns[i];

                        if (questPawn.Pawn == null || questPawn.Pawn.IsPrisoner || questPawn.Pawn.Faction.PlayerRelationKind == RimWorld.FactionRelationKind.Hostile || (WorldPawnsUtility.IsWorldPawn(questPawn.Pawn) && !questPawn.WorldQuester) || questPawn.Pawn.Dead || questPawn.Pawn.Destroyed)
                        {
                            questPawn.Destroy();
                        }
                    }
                }

                for (int i = 0; i < communications.Quests.Count; i++)
                {
                    Quest quest = communications.Quests[i];
                    try
                    {
                        quest.Tick();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception ticking quest {quest.id} --> {ex}");
                    }
                }

                for (int i = 0; i < communications.Components.Count; i++)
                {
                    var component = communications.Components[i];

                    try
                    {
                        component.Tick();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception ticking component {component.id} --> {ex}");
                    }
                }

                Communications.FactionManager.Tick();
            }
        }

        public QuestsManager(Game game)
        {
            communications = null;
            Building_Geoscape.PlayerHasGeoscape = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref communications, "communications");
        }

        public override void FinalizeInit()
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    if (map.listerBuildings.allBuildingsColonist.Any(b => b is Building_Geoscape))
                    {
                        Building_Geoscape.PlayerHasGeoscape = true;
                        break;
                    }
                }
            }

            for(int i = 0; i < Communications.Quests.Count; i++)
            {
                var quest = Communications.Quests[i];
                if (quest != null)
                {
                    quest.GameLoaded();
                }
            }
        }
    }
}
