using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public enum TraderCategory
    {
        Pawns,
        Weapon,
        Medicine,
        Other
    }

    public enum TraderType
    {
        Trader
    }

    public class DarkNetTraderDef : Def
    {
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

        [Unsaved]
        private Texture2D iconTexture;

        [NoTranslate]
        public string IconTextureInt;

        public TraderCategory Category;

        public TraderType TraderType;
    }
}
