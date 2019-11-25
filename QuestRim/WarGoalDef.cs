using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class WarGoalDef : Def
    {
        public List<AllianceGoalDef> TargetGoals;

        public Type workerClass;

        public Color MenuColor;

        [NoTranslate]
        public string attackersTexture;

        [NoTranslate]
        public string defendersTexture;

        public Texture2D AttackersTexture
        {
            get
            {
                if (attackersTexture == null)
                {
                    if (attackersTexture.NullOrEmpty())
                    {
                        return null;
                    }
                    attackersTextureInt = ContentFinder<Texture2D>.Get(attackersTexture);
                }
                return attackersTextureInt;
            }
        }

        [Unsaved]
        private Texture2D attackersTextureInt;

        public Texture2D DefendersTexture
        {
            get
            {
                if (defendersTextureInt == null)
                {
                    if (defendersTexture.NullOrEmpty())
                    {
                        return null;
                    }
                    defendersTextureInt = ContentFinder<Texture2D>.Get(defendersTexture);
                }
                return defendersTextureInt;
            }
        }

        [Unsaved]
        private Texture2D defendersTextureInt;
    }
}
