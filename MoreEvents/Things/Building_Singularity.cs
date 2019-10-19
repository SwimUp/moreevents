using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things
{
    [StaticConstructorOnStartup]
    public class Building_Singularity : Building
    {
        private int animTime = 20;
        private int maxCycles = 19;
        private string FramePath = "Things/Buildings/Singularity/";
        private static Graphic[] Frames = null;
        private int cycle = 1;
        private Graphic TexMain = null;

        public float DestablishedNum = 0f;

        public float DestablishedSpeed => 0.16f;

        private List<IntVec3> cells = new List<IntVec3>();

        public override string Label => base.Label + " (" + DestablishedNum.ToString("f2") + ")";

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            cells = GenRadial.RadialCellsAround(Position, 4, true).ToList();
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
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            GenExplosion.DoExplosion(Position, Map, 8f, DamageDefOf.Bomb, null);

            base.Destroy(mode);
        }

        public override void Tick()
        {
            base.Tick();

            if(DestablishedNum >= 100f)
            {
                Destroy();
            }

            if (Find.TickManager.TicksGame % animTime == 0)
            {
                ChangeAnim();
            }

            if (Find.TickManager.TicksGame % 10 == 0)
            {
                foreach (var c in cells)
                {
                    if (c.InBounds(Map))
                    {
                        var things = c.GetThingList(Map);
                        for(int i = 0; i < things.Count; i++)
                        {
                            Thing t = things[i];

                            if (t is Pawn pawn)
                                pawn.Position = Position;

                            if(t.def.destroyable)
                            {
                                t.TakeDamage(new DamageInfo(DamageDefOf.Crush, Rand.Range(2, 4)));
                            }
                        }
                    }
                }

                DestablishedNum += DestablishedSpeed;
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
                Vector3 s = new Vector3(3f, 2f, 3f);
                matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Rotation.AsQuat, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, this.TexMain.MatAt(Rotation, null), 0);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref DestablishedNum, "destablishedNum");
        }
    }
}
