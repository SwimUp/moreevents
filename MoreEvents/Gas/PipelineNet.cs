using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class PipelineNet
    {
        public int NetType;

        public int NetID;

        public List<ThingWithComps> PipedThings = new List<ThingWithComps>();

        public List<CompPipe> Pipes;

        public GasManager GasManager;

        public virtual void InitNet()
        {
            Pipes = GenCollection.InRandomOrder((from x in PipedThings
                                                           where x is Building_Pipe
                                                           select x).SelectMany((ThingWithComps x) => x.GetComps<CompPipe>()), (IList<CompPipe>)null).ToList();
            Tick();
        }

        public virtual void Tick()
        {
         
        }
    }
}
