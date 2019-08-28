﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class DarkNetTraderDef : Def
    {
        public DarkNetTrader workerClass;

        public Texture2D IconTexture
        {
            get
            {
                if(iconTexture == null)
                {
                    iconTexture = ContentFinder<Texture2D>.Get(IconTextureInt);
                }

                return iconTexture;
            }
        }

        public Texture2D FullTexture
        {
            get
            {
                if (fullTexture == null)
                {
                    fullTexture = ContentFinder<Texture2D>.Get(FullTextureInt);
                }

                return fullTexture;
            }
        }

        [Unsaved]
        private Texture2D iconTexture;

        [NoTranslate]
        public string IconTextureInt;

        [Unsaved]
        private Texture2D fullTexture;

        [NoTranslate]
        public string FullTextureInt;

        public List<DarkNetGood> AvaliableGoods;

        public TraderParams Character;

        public PriceModificatorFilter AllowedPriceModificatorsFilter;

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            foreach(var good in AvaliableGoods)
            {
                good.ThingFilter.ResolveReferences();
            }

            if(AllowedPriceModificatorsFilter != null)
            {
                AllowedPriceModificatorsFilter.ResolveReferences();
            }
        }
    }
}
