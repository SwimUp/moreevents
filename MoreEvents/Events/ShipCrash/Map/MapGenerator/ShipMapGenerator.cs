using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public abstract class ShipMapGenerator
    {
        public abstract ShipSiteType SiteType { get; }

        public Texture2D ExpandTexture
        {
            get
            {
                if(expandTexture == null)
                {
                    if (!TexturePath.NullOrEmpty())
                    {
                        expandTexture = ContentFinder<Texture2D>.Get(TexturePath);
                    }
                    else
                    {
                        expandTexture = BaseContent.BadTex;
                    }
                }

                return expandTexture;
            }
        }
        private Texture2D expandTexture;
        public virtual string TexturePath => @"Map/cargo_complex";

        public virtual string ExpandLabel => Translator.Translate("Ship_ExpandLabel");

        public virtual string Description => Translator.Translate("Ship_Description");

        public abstract void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner);
    }
}
