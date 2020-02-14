using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.AI
{
    public class CaravanAI_QueueAction : IExposable
    {
        public CaravanArrivalAction CaravanArrivalAction;

        public int DestinationTile;

        public CaravanAI_QueueAction()
        {

        }

        public CaravanAI_QueueAction(CaravanArrivalAction action, int tile)
        {
            CaravanArrivalAction = action;
            DestinationTile = tile;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref CaravanArrivalAction, "CaravanArrivalAction");
            Scribe_Values.Look(ref DestinationTile, "DestinationTile");
        }
    }

    public class CaravanAI : Caravan
    {
        public bool ShowNeeds = false;

        public bool ShowSocial = false;

        public bool ShowItems = false;

        public bool ShowGizmos = false;

        public bool UseFood = false;

        public CaravanAI_NeedsTracker aiNeeds;

        private Material cachedMat;

        public float Threat;

        public MapParent Home;

        public override Material Material
        {
            get
            {
                if (cachedMat == null)
                {
                    cachedMat = MaterialPool.MatFrom(color: CaravanColor, texPath: def.texture, shader: ShaderDatabase.WorldOverlayTransparentLit, renderQueue: WorldMaterials.DynamicObjectRenderQueue);
                }
                return cachedMat;
            }
        }

        public Color CaravanColor;

        public IEnumerable<CaravanAI_QueueAction> QueueActions => queueActions;
        private List<CaravanAI_QueueAction> queueActions = new List<CaravanAI_QueueAction>();

        private CaravanArrivalAction_AttackCaravanAI caravanAction;

        public CaravanAI() : base()
        {
            aiNeeds = new CaravanAI_NeedsTracker(this);
            caravanAction = new CaravanArrivalAction_AttackCaravanAI(this);
        }

        public void AddQueueAction(CaravanArrivalAction action, int destinationTile)
        {
            queueActions.Add(new CaravanAI_QueueAction(action, destinationTile));
        }

        public override string GetInspectString()
        {
            return Faction.Name;
        }

        public override void Tick()
        {
            for (int i = 0; i < AllComps.Count; i++)
            {
                AllComps[i].CompTick();
            }

            CheckAnyNonWorldPawns();
            pather.PatherTick();
            tweener.TweenerTick();
            forage.ForageTrackerTick();
            carryTracker.CarryTrackerTick();
            beds.BedsTrackerTick();
            aiNeeds.NeedsTrackerTick();
            CaravanDrugPolicyUtility.CheckTakeScheduledDrugs(this);
            CaravanTendUtility.CheckTend(this);

            if (queueActions.Count > 0 && pather.ArrivalAction == null)
            {
                var action = queueActions[queueActions.Count - 1];
                pather.StartPath(action.DestinationTile, action.CaravanArrivalAction);

                queueActions.RemoveLast();
            }

            Caravan playerCaravan = Find.WorldObjects.PlayerControlledCaravanAt(Tile);
            if (playerCaravan != null)
            {
                caravanAction.Arrived(playerCaravan);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (ShowGizmos)
            {
                foreach (var gizmo in base.GetGizmos())
                {
                    yield return gizmo;
                }
            }
            else
            {
                yield break;
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            if(Faction.PlayerRelationKind == FactionRelationKind.Hostile)
            {
                foreach(var act in CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanAttack(caravan, this), () => caravanAction, caravanAction.Label, caravan, Tile, this))
                {
                    yield return act;
                }
            }

            yield break;
        }

        public override IEnumerable<InspectTabBase> GetInspectTabs()
        {
            if (def.inspectorTabsResolved != null)
            {
                foreach(var tab in def.inspectorTabsResolved)
                {
                    if (!ShowNeeds && tab is WITab_Caravan_Needs)
                        continue;

                    if (!ShowSocial && tab is WITab_Caravan_Social)
                        continue;

                    if (!ShowItems && tab is WITab_Caravan_Items)
                        continue;

                    yield return tab;
                }
            }

            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ShowNeeds, "ShowPawns");
            Scribe_Values.Look(ref ShowSocial, "ShowSocial");
            Scribe_Values.Look(ref ShowItems, "ShowItems");
            Scribe_Values.Look(ref UseFood, "UseFood");
            Scribe_Deep.Look(ref aiNeeds, "aiNeeds", this);
            Scribe_Values.Look(ref CaravanColor, "CaravanColor");
            Scribe_Collections.Look(ref queueActions, "queueActions", LookMode.Deep);
            Scribe_Values.Look(ref Threat, "Threat");
            Scribe_References.Look(ref Home, "Home");
            Scribe_Values.Look(ref ShowGizmos, "ShowGizmos");
        }

        private void CheckAnyNonWorldPawns()
        {
            for (int num = pawns.Count - 1; num >= 0; num--)
            {
                if (!pawns[num].IsWorldPawn())
                {
                    Log.Error("Caravan member " + pawns[num] + " is not a world pawn. Removing...");
                    pawns.Remove(pawns[num]);
                }
            }
        }
    }
}
