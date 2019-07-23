using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public class RandomPlacesComponent : GameComponent
    {
        public static RandomPlacesHandler RandomPlacesHandler;

        public RandomPlacesComponent()
        {

        }

        public RandomPlacesComponent(Game game)
        {

        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            RandomPlacesHandler = new RandomPlacesHandler();

            RandomPlacesHandler.InitPlaces();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref RandomPlacesHandler, "RandomPlacesHandler");
        }
    }
}
