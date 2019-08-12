using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class PipelineNet
    {
        public PipeType NetType;

        public int NetID;

        public List<CompPipe> Pipes = new List<CompPipe>();
        public List<ThingWithComps> PipesThings = new List<ThingWithComps>();

        public GasManager GasManager;

        public void DeregisterPipe(ThingWithComps thing)
        {
            PipesThings.Remove(thing);
            InitNet();
        }

        public void InitNet()
        {
            GenList.RemoveDuplicates(PipesThings);

            Pipes = GenCollection.InRandomOrder<CompPipe>((from x in PipesThings
                                                           where x is Building_Pipe
                                                           select x).SelectMany((ThingWithComps x) => x.GetComps<CompPipe>()), null).ToList();
        }

    }
}
