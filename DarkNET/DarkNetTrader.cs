using DarkNET.TraderComp;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public abstract class DarkNetTrader : IExposable
    {
        public DarkNetTraderDef def;

        public string Name => def.label;

        public string Description => def.description;

        public Texture2D IconMenu => def.IconTexture;

        public virtual int OnlineTime => 2;
        public virtual int ArriveTime => 10;

        private int lastArriveTicks = 0;

        public bool Online;

        public TraderParams Character => def.Character; 

        public virtual bool OnlineEveryTime => false;

        private List<DarkNetComp> comps;

        public void InitializeComps()
        {
            if (def.comps.Any())
            {
                comps = new List<DarkNetComp>();
                for (int i = 0; i < def.comps.Count; i++)
                {
                    DarkNetComp darkNetComp = (DarkNetComp)Activator.CreateInstance(def.comps[i].compClass);
                    darkNetComp.parent = this;
                    comps.Add(darkNetComp);
                    darkNetComp.Initialize(def.comps[i]);
                }
            }
        }

        public T TryGetComp<T>() where T : DarkNetComp
        {
            if (comps != null)
            {
                int i = 0;
                for (int count = comps.Count; i < count; i++)
                {
                    T val = comps[i] as T;
                    if (val != null)
                    {
                        return val;
                    }
                }
            }
            return (T)null;
        }

        public virtual void FirstInit()
        {
            InitializeComps();
        }

        public virtual void Arrive()
        {
            if(DarkNet.PlayerHasDarkNetConsole)
                Find.LetterStack.ReceiveLetter($"DarkNetNotify_TraderArriveTitle".Translate(def.LabelCap), "DarkNetNotify_TraderArriveTitle".Translate(def.LabelCap), LetterDefOf.NeutralEvent);
        }

        public virtual void WindowOpen()
        {

        }

        public virtual void OnDayPassed()
        {
            if (OnlineEveryTime)
                return;

            float num = (Find.TickManager.TicksGame - lastArriveTicks) / 60000f;
            if (Online)
            {
                if (num > OnlineTime)
                {
                    Online = false;
                    lastArriveTicks = Find.TickManager.TicksGame;
                    TraderGone();
                }
            }
            else
            {
                if (num > ArriveTime)
                {
                    Online = true;
                    lastArriveTicks = Find.TickManager.TicksGame;
                    Arrive();
                }
            }
        }

        public virtual void TraderGone()
        {

        }

        public abstract void DrawTraderShop(Rect rect, Pawn speaker);

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref lastArriveTicks, "lastArriveTicks");
            Scribe_Values.Look(ref Online, "Online");

            if(Scribe.mode == LoadSaveMode.LoadingVars)
            {
                InitializeComps();
            }
        }
    }
}
