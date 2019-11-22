using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public abstract class AllianceAgreementCondition
    {
        public abstract string Reason { get; }

        public abstract bool Avaliable(Alliance alliance);
    }
}
