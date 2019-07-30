﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1
{
    public class MKStationModuleDef : Def
    {
        public string Icon;

        public ThingDef Item;

        public Texture2D IconImage
        {
            get
            {
                if(iconImage == null)
                {
                    if(!string.IsNullOrEmpty(Icon))
                    {
                        iconImage = ContentFinder<Texture2D>.Get(Icon);
                    }
                }

                return iconImage;
            }
        }
        private Texture2D iconImage;

        public float EnergyBankCharge;

        public float EnergyBankCapacity;

        public float AdditionalChargeSpeed = 0f;

        public Type workerClass;
    }
}
