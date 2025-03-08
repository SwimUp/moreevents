﻿using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EmailMessages
{
    public class MessageSender : WorldComponent
    {
        private Dictionary<EmailMessageDef, int> lastSendedMessages;

        public MessageSender(World world) : base(world)
        {
            lastSendedMessages = new Dictionary<EmailMessageDef, int>();
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            if (!Building_Geoscape.PlayerHasGeoscape)
                return;

            if(Find.TickManager.TicksGame % 5000 == 0)
            {
                if(TryFindMessage(out EmailMessageDef message))
                {
                    if (TrySendMessage(message))
                    {
                        int ticksGame = Find.TickManager.TicksGame;
                        if (lastSendedMessages.ContainsKey(message))
                        {
                            lastSendedMessages[message] = ticksGame;
                        }
                        else
                        {
                            lastSendedMessages.Add(message, ticksGame);
                        }
                    }
                }
            }
        }

        public bool TrySendMessage(EmailMessageDef msg)
        {
            EmailBox playerBox = QuestsManager.Communications.PlayerBox;

            Faction faction = null;
            if(msg.StaticFaction != null)
            {
                if(!TryGetFactionWithFilter(msg.SenderAvaliable, msg.MinReqGoodWill, msg.StaticFaction, out faction))
                {
                    return false;
                }
            }
            else
            {
                if (!TryGetFaction(msg.SenderAvaliable, msg.MinReqGoodWill, out faction))
                {
                    return false;
                }
            }

            if (faction != null)
            {
                EmailMessage message = playerBox.FormMessageFrom(faction, msg.EmailText, msg.Subject);
                message.Answers = msg.Options;

                bool canSend = true;
                if (msg.MessageWorker != null)
                {
                    canSend = msg.MessageWorker.PreReceived(message, playerBox);
                }

                if (canSend)
                {
                    playerBox.SendMessage(message);

                    msg.MessageWorker?.OnReceived(message, playerBox);

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool TryFindMessage(out EmailMessageDef message)
        {
            message = null;

            List<EmailMessageDef> messages = DefDatabase<EmailMessageDef>.AllDefsListForReading;
            List<EmailMessageDef> toSend = new List<EmailMessageDef>();

            int ticksGame = Find.TickManager.TicksGame;
            for (int i = 0; i < messages.Count; i++)
            {
                EmailMessageDef def = messages[i];

                if (GenDate.DaysPassed < def.EarliestDay)
                    continue;

                if (lastSendedMessages.TryGetValue(def, out int value))
                {
                    float num = (float)(ticksGame - value) / 60000f;
                    if (num < def.MinRefiredDays)
                        continue;
                }

                if (def.StaticFaction == null && !TryGetFaction(def.SenderAvaliable, def.MinReqGoodWill, out Faction faction))
                    continue;

                if (def.MessageWorker != null && !def.MessageWorker.CanReceiveNow())
                    continue;

                toSend.Add(def);
            }

            if (toSend.Count == 0)
                return false;

            if(toSend.TryRandomElementByWeight(w => w.Commonality, out message))
            {
                return true;
            }

            return false;
        }

        private bool TryGetFaction(FactionRelationKind kind, IntRange minReqGoodWill, out Faction faction)
        {
            if ((from f in Find.FactionManager.AllFactionsVisible where f != Faction.OfPlayer && f.PlayerRelationKind == kind && minReqGoodWill.InRange(f.PlayerGoodwill) select f).TryRandomElement(out faction))
            {
                return true;
            }
            return false;
        }

        private bool TryGetFactionWithFilter(FactionRelationKind kind, IntRange minReqGoodWill, FactionDef filter, out Faction faction)
        {
            if ((from f in Find.FactionManager.AllFactions where f != Faction.OfPlayer && f.PlayerRelationKind == kind && minReqGoodWill.InRange(f.PlayerGoodwill) && f.def == filter select f).TryRandomElement(out faction))
            {
                return true;
            }
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref lastSendedMessages, "lastSendedMessages", LookMode.Def, LookMode.Value);
        }
    }
}
