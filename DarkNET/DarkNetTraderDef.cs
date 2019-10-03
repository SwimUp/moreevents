using DarkNET.TraderComp;
using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class DarkNetTraderDef : Def
    {
        public Type workerClass;

        public float OverridePortraitWidth = 0;

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

        public List<DarkNetProperties> comps = new List<DarkNetProperties>();

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            foreach(var comp in comps)
            {
                comp.ResolveReferences();
            }

            if (AvaliableGoods != null)
            {
                foreach (var good in AvaliableGoods)
                {
                    good.ThingFilter.ResolveReferences();
                }
            }

            if(AllowedPriceModificatorsFilter != null)
            {
                AllowedPriceModificatorsFilter.ResolveReferences();
            }
        }
    }
}
