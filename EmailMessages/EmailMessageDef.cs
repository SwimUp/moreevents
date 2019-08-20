using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageDef : Def
    {
        public string EmailText;

        public string Subject;

        public FactionRelationKind SenderAvaliable;

        public int MinRefiredDays;

        public float Commonality;

        public int EarliestDay = 0;

        public IntRange MinReqGoodWill = new IntRange(-100, 100);

        public List<EmailMessageOption> Options;

        public EmailMessageWorker MessageWorker;

        public FactionDef StaticFaction;
    }
}
