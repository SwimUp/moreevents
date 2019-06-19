using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents.Things.Mk1
{
    [StaticConstructorOnStartup]
    public class Mk1PowerStation : Building
    {
        public Thing ContainedArmor;

        public bool HasArmor => ContainedArmor != null;

        private static Graphic DisableTex = null;
        private static Graphic EnableTex = null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            LongEventHandler.ExecuteWhenFinished((Action)CreateAnim);
        }

        private void CreateAnim()
        {
            DisableTex = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath, this.def.graphicData.Graphic.Shader);
            DisableTex.drawSize = this.def.graphicData.drawSize;

            EnableTex = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath + "_enable", this.def.graphicData.Graphic.Shader);
            EnableTex.drawSize = this.def.graphicData.drawSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref ContainedArmor, "ContainedArmor");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (HasArmor)
            {
                yield return new FloatMenuOption("UnloadArmor".Translate(), delegate
                {
                    Job job = new Job(JobDefOfLocal.UnLoadArmorIntoStation, this);
                    selPawn.jobs.TryTakeOrderedJob(job);
                });
            }
            else
            {
                yield return new FloatMenuOption("LoadArmor".Translate(), delegate
                {
                    var armors = this.Map.listerThings.ThingsOfDef(ThingDefOfLocal.Apparel_MK1Thunder);
                    if (armors.Count == 0)
                    {
                        Messages.Message("NoAvaliableArmors".Translate(), MessageTypeDefOf.NeutralEvent, false);
                        return;
                    }

                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (var armor in armors)
                        list.Add(new FloatMenuOption($"{armor.LabelCap}", delegate
                        {
                            Job job = new Job(JobDefOfLocal.LoadArmorIntoStation, this, armor);
                            selPawn.jobs.TryTakeOrderedJob(job);
                        }));

                    Find.WindowStack.Add(new FloatMenu(list));
                });
            }
        }

        public override void Draw()
        {
            Matrix4x4 matrix = default(Matrix4x4);
            Vector3 s = new Vector3(4f, 1f, 4f);
            matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Rotation.AsQuat, s);

            if (HasArmor && EnableTex != null)
            {
                Graphics.DrawMesh(MeshPool.plane10, matrix, EnableTex.MatAt(Rotation, null), 0);
            }else if(DisableTex != null)
            {
                Graphics.DrawMesh(MeshPool.plane10, matrix, DisableTex.MatAt(Rotation, null), 0);
            }
        }
    }
}
