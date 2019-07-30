using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailMessages
{
    public abstract class EmailMessageWorker
    {
        public abstract void OnReceived(EmailMessage message, EmailBox box);

        public virtual bool CanReceiveNow()
        {
            return true;
        }
    }
}
