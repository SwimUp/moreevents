using RimWorld;
using Verse;
using UnityEngine;

namespace MoreEvents.Weather
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_Sandstorm : SkyOverlay
    {
        private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld", -1);

        public WeatherOverlay_Sandstorm()
        {
            base.worldOverlayMat = FogOverlayWorld;
            base.worldOverlayPanSpeed1 = 0.008f;
            base.worldPanDir1 = new Vector2(-1f, -0.26f);
            base.worldPanDir1.Normalize();
            base.worldOverlayPanSpeed2 = 0.012f;
            base.worldPanDir2 = new Vector2(-1f, -0.24f);
            base.worldPanDir2.Normalize();
        }
    }
}
