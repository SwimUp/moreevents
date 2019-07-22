using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if (TryGetFaction(msg.SenderAvaliable, out Faction faction))
            {
                EmailMessage message = playerBox.FormMessageFrom(faction, msg.EmailText, msg.Subject);

                playerBox.SendMessage(message);

                return true;
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
                if (lastSendedMessages.TryGetValue(def, out int value))
                {
                    float num = (float)(ticksGame - value) / 60000f;
                    if (num < def.MinRefiredDays)
                        continue;
                }

                if(TryGetFaction(message.SenderAvaliable, out Faction faction))
                {
                    toSend.Add(message);
                }
            }

            if (toSend.Count == 0)
                return false;

            if(toSend.TryRandomElementByWeight(w => w.Commonality, out message))
            {
                return true;
            }

            return false;
        }

        private bool TryGetFaction(FactionRelationKind kind, out Faction faction)
        {
            if ((from f in Find.FactionManager.AllFactionsVisible where f.PlayerRelationKind == kind select f).TryRandomElement(out faction))
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
