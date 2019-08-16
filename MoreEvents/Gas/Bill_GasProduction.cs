using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class Bill_GasProduction : Bill_Production
    {
        public Building_GasStation Station;

        public Bill_GasProduction() : base()
        {

        }

        public Bill_GasProduction(RecipeDef recipe) : base(recipe)
        {

        }

        public Bill_GasProduction(RecipeDef recipe, Building_GasStation station) : base(recipe)
        {
            Station = station;
        }

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            if (Station != null)
            {
                float cost = Station.GasModifiers[recipe];
                Station.Storage -= cost;
            }

            base.Notify_IterationCompleted(billDoer, ingredients);
        }
    }
}
