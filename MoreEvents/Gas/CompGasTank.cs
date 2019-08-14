using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    [StaticConstructorOnStartup]
    public class CompGasTank : CompPipe
    {
        public CompProperties_GasTask Props => (CompProperties_GasTask)props;

        public bool Full => Storage == Props.StorageCapacity;

        public float Storage;

        private static readonly Vector2 BarSize = new Vector2(0.22f, 1.6f);

        private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

        private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref Storage, "Storage");
        }

        public void PushGas(CompGasCooler cooler, float count)
        {
            if (Full)
                return;

            Storage += count;
            if (Storage > Props.StorageCapacity)
            {
                Storage = Props.StorageCapacity;
                count -= Storage - Props.StorageCapacity;
            }

            cooler.Storage -= count;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = parent.DrawPos + Vector3.up * 0.1f;
            r.size = BarSize;
            r.fillPercent = Storage / Props.StorageCapacity;
            r.filledMat = PowerPlantSolarBarFilledMat;
            r.unfilledMat = PowerPlantSolarBarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = parent.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            if (DebugSettings.godMode)
            {
                builder.AppendLine(base.CompInspectStringExtra());
            }
            builder.Append("CompGasTank_Storage".Translate(Storage.ToString("f2"), Props.StorageCapacity.ToString("f2")));

            return builder.ToString();
        }
    }
}
