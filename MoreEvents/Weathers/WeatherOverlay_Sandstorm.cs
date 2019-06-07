using RimWorld;
using Verse;
using UnityEngine;

namespace MoreEvents.Weather
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_Sandstorm : SkyOverlay
    {
        private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/RainOverlayWorld", -1);

        public WeatherOverlay_Sandstorm()
        {
            worldOverlayMat = FogOverlayWorld;
            screenOverlayMat = FogOverlayWorld;
            FogOverlayWorld.color = new Color32(209, 177, 96, 255);
            OverlayColor = new Color32(209, 177, 96, 255);
            this.worldOverlayPanSpeed1 = 0.0002f;
            this.worldOverlayPanSpeed2 = 0.0001f;
            this.worldPanDir1 = new Vector2(1f, 1f);
            this.worldPanDir2 = new Vector2(1f, -1f);
        }
    }
}
