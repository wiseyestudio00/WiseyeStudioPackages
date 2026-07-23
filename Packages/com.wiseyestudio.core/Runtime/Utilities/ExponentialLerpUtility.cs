using UnityEngine;

namespace WiseyeStudio
{
    public static class ExponentialLerpUtility
    {
        /// <summary>
        /// Exponential interpolation between two positive values with a custom neutral point.
        /// At t=neutral, the result is always 1.0 (no change).
        /// </summary>
        /// <param name="a">Value at t=0. Must be positive.</param>
        /// <param name="b">Value at t=1. Must be positive.</param>
        /// <param name="t">Interpolation factor, typically 0-1.</param>
        /// <param name="neutral">The value of t where the result equals 1.</param>
        public static float ExpLerp(float a, float b, float t, float neutral)
        {
            return t <= neutral ? ExpLerp(a, 1f, t / neutral) : ExpLerp(1f, b, (t - neutral) / (1f - neutral));
        }
        
        /// <summary>
        /// Exponential interpolation between two positive values.
        /// Unlike Lerp, this interpolates in log-space so ratios are preserved.
        /// Equivalent to: a * (b/a)^t
        /// </summary>
        /// <param name="a">Start value (t=0). Must be positive.</param>
        /// <param name="b">End value (t=1). Must be positive.</param>
        /// <param name="t">Interpolation factor, typically 0-1.</param>
        public static float ExpLerp(float a, float b, float t)
        {
            return a * Mathf.Pow(b / a, t);
        }
    }
}
