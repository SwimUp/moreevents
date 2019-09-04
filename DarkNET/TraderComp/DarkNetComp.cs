using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkNET.TraderComp
{
    public class DarkNetComp
    {
        public DarkNetTrader parent;

        public DarkNetProperties props;

        public virtual void Initialize(DarkNetProperties props)
        {
            this.props = props;
        }
    }
}
