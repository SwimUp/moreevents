﻿using System;
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
                if (communications.Quests != null)
                {
                    List<Quest> quests = communications.Quests;
                    for (int i = 0; i < quests.Count; i++)
                    {
                        try
                        {
                            quests[i].Tick();
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Exception ticking {quests[i].UniqueId} --> {ex}");
                        }
                    }
                }
            }
        }

        public QuestsManager(Game game)
        {
            communications = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref communications, "communications");
        }
    }
}
