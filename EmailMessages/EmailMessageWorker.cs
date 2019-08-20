using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailMessages
{
    public abstract class EmailMessageWorker
    {
        public virtual void OnReceived(EmailMessage message, EmailBox box)
        {

        }

        public virtual bool PreReceived(EmailMessage message, EmailBox box)
        {
            return true;
        }

        public virtual bool CanReceiveNow()
        {
            return true;
        }
    }
}
