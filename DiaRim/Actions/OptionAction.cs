﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiaRim.Actions
{
    public abstract class OptionAction
    {
        public DialogOption Option;

        public abstract void DoAction();
    }
}
