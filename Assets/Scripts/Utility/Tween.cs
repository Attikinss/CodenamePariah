using UnityEngine;

namespace Tweening
{
    public static class Tween
    {
        public static float EaseIn2(float t) => t * t;
        public static float EaseIn3(float t) => t * t * t;
        public static float EaseIn4(float t) => t * t * t * t;
        public static float EaseIn5(float t) => t * t * t * t * t;

        public static float EaseOut2(float t)
        {
            float tFlip = Flip(t);
            return Flip(tFlip * tFlip);
        }
        public static float EaseOut3(float t)
        {
            float tFlip = Flip(t);
            return Flip(tFlip * tFlip * tFlip);
        }
        public static float EaseOut4(float t)
        {
            float tFlip = Flip(t);
            return Flip(tFlip * tFlip * tFlip * tFlip);
        }
        public static float EaseOut5(float t)
        {
            float tFlip = Flip(t);
            return Flip(tFlip * tFlip * tFlip * tFlip * tFlip);
        }

        public static float EaseInOut2(float t) => Lerp(EaseIn2(t), EaseOut2(t), t);
        public static float EaseInOut3(float t) => Lerp(EaseIn3(t), EaseOut3(t), t);
        public static float EaseInOut4(float t) => Lerp(EaseIn4(t), EaseOut4(t), t);
        public static float EaseInOut5(float t) => Lerp(EaseIn5(t), EaseOut5(t), t);

        public static float Arch(float t) => Scale(Flip(t));
        public static float EaseInArch(float t) => EaseIn2(Arch(t));
        public static float EaseOutArch(float t) => EaseOut2(Arch(t));
        public static float BellCurve(float t) => EaseOut5(t) * EaseIn5(t);

        public static float Spike(float t)
        {
            if (t <= 0.5f)
                return EaseIn3(t / 0.5f);

            return EaseIn3(Flip(t) / 0.5f);
        }

        public static float Scale(float t) => Square(t);
        public static float ReverseScale(float t) => Flip(Scale(t));
        public static float Square(float val) => val * val;
        public static float Flip(float val) => 1.0f - val;
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
