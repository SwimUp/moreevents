using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class DarkNetProperties
    {
        [TranslationHandle]
        public Type compClass;

        public DarkNetProperties()
        {

        }

        public DarkNetProperties(Type compClass)
        {
            this.compClass = compClass;
        }

        public virtual void ResolveReferences()
        {

        }
    }
}
