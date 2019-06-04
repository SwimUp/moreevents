using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class ShipCargo_Food : Ship_Cargo
    {
        public override CargoType PartType => CargoType.Food;
        public override string texturePath => @"Map/cargo_food";

        public override string ExpandLabel => Translator.Translate("ShipCargo_Food_ExpandLabel");

        public override string Description => Translator.Translate("ShipCargo_Food_Description");

        private const int minSupply = 15;
        private const int maxSupply = 40;

        private bool dangerous = true;

        public override void RunGenerator()
        {
            Log.Message("RUN");
        }
    }
}
