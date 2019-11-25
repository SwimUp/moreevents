using QuestRim;
using RimOverhaul.AI;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Alliances
{
    public class TradeSchedulingAgreementComp : AllianceAgreementComp
    {
        public TradeSchedulingAgreementCompProperties TradeSchedulingAgreementCompProperties => (TradeSchedulingAgreementCompProperties)props;

        public List<Thing> Items;

        public CaravanAI Caravan;

        public Settlement Settlement;

        public Map DestinationMap;

        public bool UsedCaravan = false;

        private int formCaravanTicks;

        private bool caravanFormed = false;

        public TradeSchedulingAgreementComp()
        {

        }

        public TradeSchedulingAgreementComp(Alliance alliance, FactionInteraction signer, List<Thing> items, Settlement settlement, Map destination, int ticksDelay, int formCaravanDelay)
        {
            AllianceAgreementDef = AllianceAgreementDefOfLocal.TradeSchedulingAgreement;

            PlayerOwner = true;
            OwnerFaction = QuestsManager.Communications.FactionManager.GetInteraction(Faction.OfPlayer);
            SignedFaction = signer;
            Items = items;
            SignTicks = Find.TickManager.TicksGame;
            EndTicks = SignTicks + ticksDelay;
            Alliance = alliance;

            DestinationMap = destination;

            Settlement = settlement;

            int factionTechLevel = (int)SignedFaction.Faction.def.techLevel;
            if (factionTechLevel < 3)
            {
                formCaravanTicks = SignTicks + (int)(formCaravanDelay * 0.97f); //because end so early

                UsedCaravan = true;
            }
            else
            {
                Items.ForEach(x =>
                {
                    if (x is Pawn pawn)
                    {
                        Find.WorldPawns.RemovePawn(pawn);
                    }
                });
            }
        }

        public override void Tick()
        {
            base.Tick();

            if(UsedCaravan)
            {
                if(DestinationMap == null && Caravan != null)
                {
                    Find.WorldObjects.Remove(Caravan);

                    End(AgreementEndReason.Force);
                }

                if (!caravanFormed)
                {
                    if (Find.TickManager.TicksGame >= formCaravanTicks)
                    {
                        caravanFormed = true;
                        CaravanDelivery();
                    }
                }
            }
        }

        public override void End(AgreementEndReason agreementEndReason)
        {
            if (agreementEndReason == AgreementEndReason.Time)
            {
                int factionTechLevel = (int)SignedFaction.Faction.def.techLevel;
                if (factionTechLevel >= 3)
                {
                    CapsuleDelivery();
                }
            }

            base.End(agreementEndReason);
        }

        private void CapsuleDelivery()
        {
            Map map = Find.AnyPlayerHomeMap;

            IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.DropThingsNear(intVec, map, Items, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

            Find.LetterStack.ReceiveLetter("DeliveryItems_Title".Translate(), "DeliveryItems_Desc".Translate(), LetterDefOf.PositiveEvent, new LookTargets(intVec, map));
        }

        private void CaravanDelivery()
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = SignedFaction.Faction,
                groupKind = PawnGroupKindDefOf.Trader,
                points = Rand.Range(300, 1000),
                tile = Settlement.Tile
            };

            Caravan = CaravanAIMaker.MakeCaravan(PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms), SignedFaction.Faction, Settlement.Tile, true, true, true);
            TryGetRandomPawnGroupMaker(pawnGroupMakerParms, out PawnGroupMaker maker);

            foreach (var p in Caravan.PawnsListForReading)
            {
                p.inventory.innerContainer.Clear();
            }

            GenerateCarriers(pawnGroupMakerParms, maker, Items, Caravan);

            Items.ForEach(item =>
            {
                if (item is Pawn p)
                {
                    Find.WorldPawns.PassToWorld(p);
                    Caravan.AddPawn(p, false);
                }
            });
            Items.Clear();

            Caravan.pather.StartPath(DestinationMap.Tile, new CaravanArrivalAction_AIEnterCaravan(DestinationMap.Parent));

            Find.LetterStack.ReceiveLetter("TradeSchedulingAgreementComp_CaravanFormedTitle".Translate(), "TradeSchedulingAgreementComp_CaravanFormedDesc".Translate(), LetterDefOf.PositiveEvent, new LookTargets(Caravan));
        }

        private bool TryGetRandomPawnGroupMaker(PawnGroupMakerParms parms, out PawnGroupMaker pawnGroupMaker)
        {
            if (parms.seed.HasValue)
            {
                Rand.PushState(parms.seed.Value);
            }
            IEnumerable<PawnGroupMaker> source = parms.faction.def.pawnGroupMakers.Where((PawnGroupMaker gm) => gm.kindDef == parms.groupKind && gm.CanGenerateFrom(parms));
            bool result = source.TryRandomElementByWeight((PawnGroupMaker gm) => gm.commonality, out pawnGroupMaker);
            if (parms.seed.HasValue)
            {
                Rand.PopState();
            }
            return result;
        }

        private void GenerateCarriers(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, List<Thing> wares, Caravan caravan)
        {
            List<Thing> list = wares.Where((Thing x) => !(x is Pawn)).ToList();
            int i = 0;
            int num = Mathf.CeilToInt((float)list.Count / 8f);
            PawnKindDef kind = groupMaker.carriers.Where((PawnGenOption x) => parms.tile == -1 || Find.WorldGrid[parms.tile].biome.IsPackAnimalAllowed(x.kind.race)).RandomElementByWeight((PawnGenOption x) => x.selectionWeight).kind;
            List<Pawn> list2 = new List<Pawn>();
            PawnGenerationRequest request = default(PawnGenerationRequest);
            for (int j = 0; j < num; j++)
            {
                PawnKindDef kind2 = kind;
                Faction faction = parms.faction;
                int tile = parms.tile;
                request = new PawnGenerationRequest(kind2, faction, PawnGenerationContext.NonPlayer, tile, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, parms.inhabitants);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                if (i < list.Count)
                {
                    pawn.inventory.innerContainer.TryAdd(list[i]);
                    i++;
                }
                list2.Add(pawn);

                Find.WorldPawns.PassToWorld(pawn);
                caravan.AddPawn(pawn, false);
            }
            for (; i < list.Count; i++)
            {
                list2.RandomElement().inventory.innerContainer.TryAdd(list[i]);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref Items, "Items", LookMode.Deep);
            Scribe_References.Look(ref Caravan, "Caravan");
            Scribe_References.Look(ref Settlement, "Settlement");
            Scribe_Values.Look(ref UsedCaravan, "UsedCaravan");
            Scribe_References.Look(ref DestinationMap, "DestinationMap");
            Scribe_Values.Look(ref formCaravanTicks, "formCaravanTicks");
            Scribe_Values.Look(ref caravanFormed, "caravanFormed");
        }
    }
}
