using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public enum AgreementCategory : byte
    {
        Millitary = 0,
        Economy = 1,
        General = 2
    }

    public class AllianceAgreementDef : Def
    {
        public bool PlayerOnly;

        public AllianceAgreementCompProperties Comp;

        public List<AllianceGoalDef> TargetGoals;

        public AgreementCategory AgreementCategory;

        public int MinMembersRequired = 1;

        public bool UseAgreementsSlot;

        [NoTranslate]
        public string agreementMenuTexture;

        [NoTranslate]
        public string offlineAgreementMenuTexture;

        public Texture2D AgreementMenuTexture
        {
            get
            {
                if (agreementMenuTextureInt == null)
                {
                    if (agreementMenuTexture.NullOrEmpty())
                    {
                        return null;
                    }
                    agreementMenuTextureInt = ContentFinder<Texture2D>.Get(agreementMenuTexture);
                }
                return agreementMenuTextureInt;
            }
        }

        [Unsaved]
        private Texture2D agreementMenuTextureInt;

        public Texture2D OfflineAgreementMenuTexture
        {
            get
            {
                if (offlineAgreementMenuTextureInt == null)
                {
                    if (offlineAgreementMenuTexture.NullOrEmpty())
                    {
                        return null;
                    }
                    offlineAgreementMenuTextureInt = ContentFinder<Texture2D>.Get(offlineAgreementMenuTexture);
                }
                return offlineAgreementMenuTextureInt;
            }
        }

        [Unsaved]
        private Texture2D offlineAgreementMenuTextureInt;

        public List<AllianceAgreementCondition> Conditions;
    }
}
