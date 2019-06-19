using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things
{
    [StaticConstructorOnStartup]
    public class Building_SingularReactor : Building
    {
        private int animTime = 6;
        private int maxCycles = 17;
        private string FramePath = "Things/Buildings/BlackholeReactor/";
        private Graphic DisableTex = null;
        private static Graphic[] Frames = null;
        private int cycle = 1;
        private Graphic TexMain = null;

        public CompPowerTrader compPowerTrader;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            compPowerTrader = GetComp<CompPowerTrader>();

            LongEventHandler.ExecuteWhenFinished((Action)CreateAnim);
        }

        private void CreateAnim()
        {
            Frames = new Graphic_Single[maxCycles];
            for (int i = 0; i < maxCycles; i++)
            {
                Frames[i] = GraphicDatabase.Get<Graphic_Single>(FramePath + (i + 1), this.def.graphicData.Graphic.Shader);
                Frames[i].drawSize = this.def.graphicData.drawSize;
            }
            DisableTex = GraphicDatabase.Get<Graphic_Single>(FramePath + 0, this.def.graphicData.Graphic.Shader);
            DisableTex.drawSize = this.def.graphicData.drawSize;
        }

        public override void Tick()
        {
            base.Tick();

            if (compPowerTrader.PowerOutput == 0)
            {
                TexMain = DisableTex;
                return;
            }

            if (Find.TickManager.TicksGame % animTime == 0)
            {
                ChangeAnim();
            }
        }
        private void ChangeAnim()
        {
            if (cycle >= maxCycles)
                cycle = 0;

            TexMain = Frames[cycle];
            TexMain.color = base.Graphic.color;
            cycle++;
        }

        public override void Draw()
        {
            if (this.TexMain != null)
            {
                Matrix4x4 matrix = default(Matrix4x4);
                Vector3 s = new Vector3(10f, 2f, 10f);
                matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Rotation.AsQuat, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, this.TexMain.MatAt(Rotation, null), 0);
            }
        }
    }
}
