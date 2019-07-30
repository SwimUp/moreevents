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
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (communications != null)
            {
                if(Find.TickManager.TicksGame % 500 == 0)
                {
                    for(int i = 0; i < communications.QuestPawns.Count; i++)
                    {
                        QuestPawn questPawn = communications.QuestPawns[i];
                        if(questPawn.Pawn == null || (WorldPawnsUtility.IsWorldPawn(questPawn.Pawn) && !questPawn.WorldQuester) || questPawn.Pawn.Dead || questPawn.Pawn.Destroyed)
                        {
                            questPawn.Destroy();
                        }
                    }
                }

                List<Quest> quests = communications.Quests;
                for (int i = 0; i < quests.Count; i++)
                {
                    try
                    {
                        quests[i].Tick();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception ticking {quests[i].id} --> {ex}");
                    }
                }

                List<CommunicationComponent> components = communications.Components;
                for (int i = 0; i < components.Count; i++)
                {
                    try
                    {
                        components[i].Tick();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception ticking {components[i].id} --> {ex}");
                    }
                }
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
        }
    }
}
