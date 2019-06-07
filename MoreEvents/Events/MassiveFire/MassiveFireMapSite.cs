using MoreEvents.Biomes;
using MoreEvents.Events.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.MassiveFire
{
    public class MassiveFireMapSite : MapParent
    {
        public override Material Material
        {
            get
            {
                if (material == null)
                {
                    material = MaterialPool.MatFrom(textures[fireLevel], ShaderDatabase.WorldOverlayTransparentLit, WorldMaterials.WorldObjectRenderQueue);
                }
                return material;
            }
        }
        private Material material;

        private static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        private string[] textures = new string[]
        {
            @"Map/MassiveFireIcon3",
            @"Map/MassiveFireIcon2",
            @"Map/MassiveFireIcon1",
        };
        private int fireLevel => comp.FireLevel;
        public List<int> Candidates = new List<int>();

        private MassiveFireComp comp;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            comp = this.GetComponent<MassiveFireComp>();
            comp.site = this;
        }

        public void ResetMaterial()
        {
            material = MaterialPool.MatFrom(textures[fireLevel], ShaderDatabase.WorldOverlayTransparentLit, WorldMaterials.WorldObjectRenderQueue);
        }

        public override void Draw()
        {
            float averageTileSize = Find.WorldGrid.averageTileSize;
            float transitionPct = ExpandableWorldObjectsUtility.TransitionPct;
            if (def.expandingIcon && transitionPct > 0f)
            {
                Color color = Material.color;
                float num = 1f - transitionPct;
                propertyBlock.SetColor(ShaderPropertyIDs.Color, new Color(color.r, color.g, color.b, color.a * num));
                Vector3 drawPos = DrawPos;
                float size = 0.7f * averageTileSize;
                float altOffset = 0.015f;
                Material material = Material;
                MaterialPropertyBlock materialPropertyBlock = propertyBlock;
                WorldRendererUtility.DrawQuadTangentialToPlanet(drawPos, size, altOffset, material, counterClockwise: false, useSkyboxLayer: false, materialPropertyBlock);
            }
            else
            {
                WorldRendererUtility.DrawQuadTangentialToPlanet(DrawPos, 0.7f * averageTileSize, 0.015f, Material);
            }
        }

        public void BurnedIt()
        {
            Tile tile = Find.WorldGrid[Tile];
            tile.biome = BiomesDefOfLocal.BurnedForest;

            Map map = Current.Game.FindMap(Tile);
            if (map != null)
            {
                Find.Maps.Remove(map);
            }

            for(int i = 0; i < Candidates.Count; i++)
            {
                if (Find.WorldObjects.AnyMapParentAt(Candidates[i]))
                    Candidates.RemoveAt(i);
            }

            if(Candidates.Count == 0)
            {
                Find.WorldObjects.Remove(this);
                return;
            }

            int nextTile = Candidates.RandomElement();
            Candidates.Remove(nextTile);
            MassiveFireMapSite site = (MassiveFireMapSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.MassiveFireSite);
            site.Candidates = Candidates;
            site.Tile = nextTile;
            Find.WorldObjects.Add(site);

            Find.WorldObjects.Remove(this);
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            comp.Stop();

            SetBurn();
        }

        private void SetBurn()
        {
            Map map = Current.Game.FindMap(Tile);
            List<IntVec3> positions = map.AllCells.Where(vec => !vec.Fogged(map) && vec.Walkable(map)).ToList();
            int count = 0;

            if (positions.Count == 0)
                return;

            if(positions.Count > 10)
            {
                count = Rand.Range(1, positions.Count / 4) + ((fireLevel + 1)* 10);
            }
            if (count > positions.Count)
                count = positions.Count;

            for(int i = 0; i < count; i++)
            {
                IntVec3 pos = positions.RandomElement();

                FireUtility.TryStartFireIn(pos, map, Rand.Range(0.1f, 0.925f));
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(caravan))
            {
                yield return floatMenuOption;
            }

            foreach (FloatMenuOption floatMenuOption2 in GetFloatMenuOptions(caravan, this))
            {
                yield return floatMenuOption2;
            }
        }

        private IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CaravanArrivalAction_EnterToMassiveFire.CanVisit(caravan, mapParent), () => new CaravanArrivalAction_EnterToMassiveFire(mapParent, this), "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (base.HasMap)
            {
                yield return LeaveCommand(base.Map);
            }
        }

        private Command LeaveCommand(Verse.Map map)
        {
            Command_Action command = new Command_Action();
            command.defaultLabel = Translator.Translate("ShipSite_LeaveCommandLabel");
            command.defaultDesc = Translator.Translate("ShipSite_LeaveCommandDesc");
            command.icon = ContentFinder<Texture2D>.Get("Map/leaving-queue");
            command.action = delegate
            {
                ForceReform(this);
            };

            if (map.mapPawns.FreeColonistsCount == 0)
            {
                command.Disable();
            }

            return command;
        }

        public void ForceReform(MapParent mapParent)
        {
            if (Dialog_FormCaravan.AllSendablePawns(mapParent.Map, reform: true).Any((Pawn x) => x.IsColonist))
            {
                Messages.Message("MessageYouHaveToReformCaravanNow".Translate(), new GlobalTargetInfo(mapParent.Tile), MessageTypeDefOf.NeutralEvent);
                Current.Game.CurrentMap = mapParent.Map;
                Dialog_FormCaravan window = new Dialog_FormCaravan(mapParent.Map, reform: true, delegate
                {
                    if(HasFire())
                    {
                        comp.Start();
                    }
                    else
                    {
                        if (mapParent.HasMap)
                        {
                            Find.WorldObjects.Remove(mapParent);
                        }
                    }

                }, mapAboutToBeRemoved: true);
                Find.WindowStack.Add(window);
                return;
            }
            List<Pawn> tmpPawns = new List<Pawn>();
            tmpPawns.Clear();
            tmpPawns.AddRange(from x in mapParent.Map.mapPawns.AllPawns
                              where x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer
                              select x);
            if (tmpPawns.Any((Pawn x) => CaravanUtility.IsOwner(x, Faction.OfPlayer)))
            {
                CaravanExitMapUtility.ExitMapAndCreateCaravan(tmpPawns, Faction.OfPlayer, mapParent.Tile, mapParent.Tile, -1);
            }
            tmpPawns.Clear();

            if (HasFire())
            {
                comp.Start();
            }
            else
            {
                if (mapParent.HasMap)
                {
                    Find.WorldObjects.Remove(mapParent);
                }
            }
        }

        private bool HasFire()
        {
            foreach(var thing in Map.spawnedThings)
            {
                if (thing.IsBurning())
                    return true;
            }

            return false;
        }
    }
}
