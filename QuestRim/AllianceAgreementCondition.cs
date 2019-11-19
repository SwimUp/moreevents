using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public class AllianceAgreementCondition
    {
        public virtual bool Avaliable(Alliance alliance)
        {
            return true;
        }
    }
}
