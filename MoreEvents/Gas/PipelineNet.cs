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
        public Dictionary<PipeType, List<CompPipe>> PipesByType;

        private List<CompPipe> thingsForTick;

        public List<ThingWithComps> PipesThings = new List<ThingWithComps>();

        public List<GasPlant> GasPlants;
        public List<CompGasCooler> GasCoolers;

        public GasManager GasManager;

        public bool Enabled;

        public void DeregisterPipe(ThingWithComps thing)
        {
            PipesThings.Remove(thing);
            InitNet();
        }

        public void InitNet()
        {
            GenList.RemoveDuplicates(PipesThings);

            PipesThings.ForEach(x => x.GetComp<CompPipe>().NetInit());

            PipesByType = new Dictionary<PipeType, List<CompPipe>>();
            thingsForTick = new List<CompPipe>();
            GasPlants = new List<GasPlant>();
            GasCoolers = new List<CompGasCooler>();

            Pipes = PipesThings.Where(x => x is Building_Pipe).Select(x2 => x2.GetComp<CompPipe>()).ToList();
            GasPlants = PipesThings.Where(x => x.TryGetComp<GasPlant>() != null).Select(x2 => x2.GetComp<GasPlant>()).ToList();
            GasCoolers = PipesThings.Where(x => x.TryGetComp<CompGasCooler>() != null).Select(x2 => x2.GetComp<CompGasCooler>()).ToList();

            foreach (PipeType type in Enum.GetValues(typeof(PipeType)))
            {
                List<CompPipe> pipes = Pipes.Where(p => p.PipeType == type).ToList();
                PipesByType.Add(type, pipes);
            }

            thingsForTick = PipesThings.Where(x => x.def.tickerType != TickerType.Never).Select(x2 => x2.GetComp<CompPipe>()).ToList();

          //  if(GasCoolers.Any(cool))
        }

        public virtual void PipelineNetTick()
        {
            if (!Enabled)
                return;

            for(int i = 0; i < thingsForTick.Count; i++)
            {
                thingsForTick[i].PipelineNet();
            }
        }

    }
}
