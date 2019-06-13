﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public abstract class ShipMapGenerator : IExposable
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
        public abstract string TexturePath { get; }

        public abstract string ExpandLabel { get; }

        public abstract string Description { get; }

        public abstract void RunGenerator(ShipCrashWorker main, Verse.Map map, Faction owner);
    }
}
