using RimWorld;
using Verse;
using UnityEngine;

namespace RimOverhaul.Weather
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_HardFog : SkyOverlay
    {
        private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld", -1);

        public WeatherOverlay_HardFog()
        {
            FogOverlayWorld.color = new Color(0.72f, 0.86f, 0.97f, 0.6f);
            this.worldOverlayMat = WeatherOverlay_HardFog.FogOverlayWorld;
            //OverlayColor = new Color(0.72f, 0.86f, 0.97f, 0.6f);
            this.worldOverlayPanSpeed1 = 0.0002f;
            this.worldOverlayPanSpeed2 = 0.0001f;
            this.worldPanDir1 = new Vector2(1f, 1f);
            this.worldPanDir2 = new Vector2(1f, -1f);
        }
    }
}
