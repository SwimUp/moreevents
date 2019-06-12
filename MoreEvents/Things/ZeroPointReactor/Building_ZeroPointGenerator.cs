using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things.ZeroPointReactor
{
    public class Building_ZeroPointGenerator : Building
    {
        private int animTime = 7;
        private int maxCycles = 10;
        private string FramePath = "Things/Buildings/ZeroPointReactor/";
        private Graphic DisableTex = null;
        private Graphic[] Frames = null;
        private int cycle = 1;
        private Graphic TexMain = null;

        public CompPowerTrader compPowerTrader;

        private List<IntVec3> affectedCells = new List<IntVec3>();

        private bool doDestroy = false;
        private float destablishedNum = 0f;

        private List<Building_VaccumPump> pumps = new List<Building_VaccumPump>();
        private bool isEnable {
            get
            {
                if (!TryGetRoom())
                    return false;

                if (cachedRoom.OpenRoofCount > 0)
                    return false;

                if(pumps.Count < 4)
                {
                    return false;
                }

                return true;
            }
        }
        private Room cachedRoom = null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            affectedCells.Clear();
            destablishedNum = 0f;

            compPowerTrader = GetComp<CompPowerTrader>();

            CreateAnim();

            Notify_PumpsChange();
        }

        public override string GetInspectString()
        {
            StringBuilder builder = new StringBuilder();
            if(!isEnable)
            {
                builder.Append(Translator.Translate("ZPRNoActiveLabel"));
                if (cachedRoom?.Role == RoomRoleDefOf.None)
                {
                    builder.Append(Translator.Translate("ZPRNeedRoom"));
                }
                if (cachedRoom?.OpenRoofCount > 0)
                {
                    builder.Append(Translator.Translate("ZPRHasRoofs"));
                }
                if (pumps.Count < 4)
                {
                    builder.Append($"{Translator.Translate("ZPRNeedMorePumps")} {pumps.Count}/4");
                }
            }
            else
            {
                builder.Append($"{Translator.Translate("ZPRWorking")}");
            }

            if(!string.IsNullOrEmpty(base.GetInspectString()))
                builder.Append($"\n{base.GetInspectString()}");

            return builder.ToString();
        }

        private bool TryGetRoom()
        {
            cachedRoom = this.GetRoom(RegionType.Set_All);

            if (cachedRoom == null || cachedRoom.Role == RoomRoleDefOf.None)
                return false;

            Notify_PumpsChange();

            return true;
        }

        public void Notify_PumpsChange()
        {
            pumps.Clear();

            if (cachedRoom == null || cachedRoom.Role == RoomRoleDefOf.None)
                return;

            var things = cachedRoom.ContainedAndAdjacentThings.Where(t => t is Building_VaccumPump).Take(4);
            foreach (var t in things)
                pumps.Add((Building_VaccumPump)t);

        }

        public void Notify_RoomChange()
        {
            cachedRoom = this.GetRoom(RegionType.Set_All);

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

            if (!isEnable || compPowerTrader.PowerOutput == 0)
            {
                TexMain = DisableTex;
                compPowerTrader.PowerOutput = 0;
                return;
            }

            if (doDestroy)
            {
                if(destablishedNum >= 100f)
                {
                    Destroy();
                }

                if (Find.TickManager.TicksGame % 10 == 0)
                {
                    foreach (var c in affectedCells)
                    {
                        if (c.InBounds(Map))
                        {
                            var pawn = c.GetFirstPawn(Map);
                            if (pawn != null)
                            {
                                pawn.Position = Position;
                                if (Rand.Chance(0.2f))
                                    pawn.TakeDamage(new DamageInfo(DamageDefOf.Crush, Rand.Range(1, 2), hitPart: GetLegs(pawn)));
                            }
                        }
                    }

                    destablishedNum += 2.8f;
                }
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
                Vector3 s = new Vector3(4f, 2f, 10f);
                matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Rotation.AsQuat, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, this.TexMain.MatAt(Rotation, null), 0);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (!doDestroy && compPowerTrader.PowerOutput > 0)
            {
                Utility.CellsToHit(Position, Map, 17f, ref affectedCells);
                animTime = 1;
                doDestroy = true;

                Find.LetterStack.ReceiveLetter(Translator.Translate("ZeroPointReactorDestablished"), Translator.Translate("ZeroPointReactorDestablishedDesc"), LetterDefOf.NegativeEvent);

                return;
            }

            GenExplosion.DoExplosion(Position, Map, 6f, DamageDefOf.Bomb, this, Rand.Range(20, 100));

            base.Destroy(mode);
        }

        private BodyPartRecord GetLegs(Pawn p)
        {
            foreach (BodyPartRecord notMissingPart in p.health.hediffSet.GetNotMissingParts())
            {
                if (notMissingPart.groups.Contains(BodyPartGroupDefOf.Legs))
                {
                    return notMissingPart;
                }
            }
            return null;
        }
    }
}
