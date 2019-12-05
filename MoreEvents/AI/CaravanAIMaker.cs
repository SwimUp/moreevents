using QuestRim;
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
    public class CaravanAIMaker
    {
        private static List<Pawn> tmpPawns = new List<Pawn>();

        public static Color GetCaravanColor(Faction caravanOwner, Faction target)
        {
            switch(caravanOwner.RelationKindWith(target))
            {
                case FactionRelationKind.Hostile:
                    return new Color(1, 0, 0);
                case FactionRelationKind.Ally:
                    return new Color(0, 1, 0);
                case FactionRelationKind.Neutral:
                    return new Color(1f, 0.863f, 0.33f);
                default:
                    return new Color(1, 1, 1);
            }
        }

        public static CaravanAI MakeCaravan(IEnumerable<Pawn> pawns, Faction faction, int startingTile, bool addToWorldPawnsIfNotAlready, Color color, bool showNeeds = false, bool showSocial = false, bool useFood = false)
        {
            if (startingTile < 0 && addToWorldPawnsIfNotAlready)
            {
                Log.Warning("Tried to create a caravan but chose not to spawn a caravan but pass pawns to world. This can cause bugs because pawns can be discarded.");
            }
            tmpPawns.Clear();
            tmpPawns.AddRange(pawns);
            CaravanAI caravan = (CaravanAI)WorldObjectMaker.MakeWorldObject(MoreEvents.Events.WorldObjectsDefOfLocal.CaravanAI);
            if (startingTile >= 0)
            {
                caravan.Tile = startingTile;
            }
            caravan.SetFaction(faction);
            if (startingTile >= 0)
            {
                Find.WorldObjects.Add(caravan);
            }
            for (int i = 0; i < tmpPawns.Count; i++)
            {
                Pawn pawn = tmpPawns[i];
                if (pawn.Dead)
                {
                    Log.Warning("Tried to form a caravan with a dead pawn " + pawn);
                    continue;
                }
                
                caravan.AddPawn(pawn, addToWorldPawnsIfNotAlready);
                if (addToWorldPawnsIfNotAlready && !pawn.IsWorldPawn())
                {
                    Find.WorldPawns.PassToWorld(pawn);
                }
            }
            caravan.Name = CaravanNameGenerator.GenerateCaravanName(caravan);
            tmpPawns.Clear();
            caravan.SetUniqueId(Find.UniqueIDsManager.GetNextCaravanID());

            caravan.ShowSocial = showSocial;
            caravan.ShowNeeds = showNeeds;
            caravan.UseFood = useFood;

            caravan.CaravanColor = color;

            return caravan;
        }
    }
}
