using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Application.ShopSystem
{
    [Serializable]
    public class PurchaseFailedShakeParameters
    {
        [Min(0.01f)] public float ShakeDuration = 0.2f;
        [Min(0.01f)] public float Strength = 20f;
        [Min(0.01f)] public float Randomness = 90f;
        public int Vibrato = 100;
        public bool Snapping;
        public bool FadeOut = true;
        public ShakeRandomnessMode ShakeRandomnessMode = ShakeRandomnessMode.Harmonic;
    }
}