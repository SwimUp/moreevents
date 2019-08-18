using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class PipelineNet
    {
        public PipeType NetType;

        public List<CompPipe> Pipes = new List<CompPipe>();

        public List<ThingWithComps> PipesThings = new List<ThingWithComps>();

        public List<GasPlant> GasPlants = new List<GasPlant>();
        public List<CompGasCooler> GasCoolers = new List<CompGasCooler>();
        public List<CompGasTank> GasTankers = new List<CompGasTank>();

        public GasManager GasManager;

        public bool CanPush = false;

        public float TotalLiquidGasNow => GasTankers.Sum(x => x.Storage);

        public bool HasLiquidGas => TotalLiquidGasNow > 0;

        public void InitNet()
        {
            GenList.RemoveDuplicates(PipesThings);

            foreach (var thing in PipesThings)
            {
                Pipes.Add(thing.GetComp<CompPipe>());

                GasPlant gasPlant = thing.TryGetComp<GasPlant>();
                if (gasPlant != null)
                {
                    GasPlants.Add(gasPlant);
                }

                CompGasCooler compGasCooler = thing.TryGetComp<CompGasCooler>();
                if(compGasCooler != null)
                {
                    GasCoolers.Add(compGasCooler);
                }

                CompGasTank compGasTank = thing.TryGetComp<CompGasTank>();
                if(compGasTank != null)
                {
                    GasTankers.Add(compGasTank);
                }
            }

            if (GasCoolers.Count > 0)
                CanPush = true;

            PipesThings.ForEach(x => x.GetComp<CompPipe>().NetInit());
        }

        public void PushGasIntoNet(GasPlant plant, float count)
        {
            if (!CanPush)
                return;

            List<CompGasCooler> notFuel = GasCoolers.Where(x => !x.Full).ToList();
            float toPush = count / notFuel.Count;
            foreach (var cooler in notFuel)
            {
                cooler.PushGas(plant, toPush);
            }

            return;
        }

        public bool GetGasFromNet(float count)
        {
            if (TotalLiquidGasNow < count)
                return false;

            float value = count;
            foreach(var tank in GasTankers.InRandomOrder())
            {
                float num = Mathf.Min(tank.Storage, value);
                tank.Storage -= num;
                value -= num;

                if(num <= 0)
                {
                    break;
                }
            }

            return true;
        }
    }
}
