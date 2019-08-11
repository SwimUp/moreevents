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
    public class Building_WorkTable_Animated : Building_WorkTable
    {
        public bool DisabledGraphicOn;

        public bool ManuallyOperated
        {
            get
            {
                if (Map == null)
                {
                    return false;
                }

                Pawn val = Map.thingGrid.ThingAt<Pawn>(InteractionCell);
                if (val != null && !val.pather.Moving && val.CurJob != null && val.CurJob.targetA != LocalTargetInfo.Invalid && val.CurJob.targetA.HasThing && val.CurJob.targetA.Thing == this)
                {
                    return val.RaceProps.Humanlike;
                }
                return false;
            }
        }

        public string GraphicOnPath => base.def.graphic.path + "_on";

        private Graphic graphic;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LongEventHandler.ExecuteWhenFinished((Action)FindTexture);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DisabledGraphicOn, "DisabledGraphicOn", true, true);
        }

        public void FindTexture()
        {
            Texture2D val = ContentFinder<Texture2D>.Get(GraphicOnPath + "_north", false);
            DisabledGraphicOn = ((val == null) ? true : false);

            graphic = GraphicDatabase.Get<Graphic_Multi>(GraphicOnPath, base.def.graphic.Shader, base.def.graphic.drawSize, base.def.graphic.Color);
        }

        public override void Draw()
        {
            base.Draw();
            if (DisabledGraphicOn || !ManuallyOperated)
            {
                return;
            }

            if (graphic != null)
            {
                Graphic obj = GraphicColoredFor(this, graphic);
                Mesh val2 = obj.MeshAt(Rotation);
                Material val3 = obj.MatAt(Rotation, null);
                if (base.def.graphic is Graphic_Single)
                {
                    Mesh obj2 = val2;
                    Vector3 val4 = this.DrawPos + Altitudes.AltIncVect;
                    Rot4 rotation = this.Rotation;
                    Graphics.DrawMesh(obj2, val4, rotation.AsQuat, val3, 0);
                }
                if (base.def.graphic is Graphic_Multi)
                {
                    Graphics.DrawMesh(val2, this.DrawPos + Altitudes.AltIncVect, Quaternion.identity, val3, 0);
                }
            }
        }

        private Graphic GraphicColoredFor(Thing t, Graphic graphic)
        {
            if (GenColor.IndistinguishableFrom(t.DrawColor, graphic.Color) && GenColor.IndistinguishableFrom(t.DrawColorTwo, graphic.ColorTwo))
            {
                return graphic;
            }
            return graphic.GetColoredVersion(t.def.graphic.Shader, t.DrawColor, t.DrawColorTwo);
        }
    }

}
