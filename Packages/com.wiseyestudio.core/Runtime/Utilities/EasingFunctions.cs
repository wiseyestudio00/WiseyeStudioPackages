using System;
using System.Collections.Generic;
using UnityEngine;

/*
* Originally from markv12: https://github.com/markv12/ManagingCoroutines
*/
namespace WiseyeStudio
{
    public static class EasingFunctions
    {
        public delegate float Easing(float start, float end, float duration);

        public static class EasingStrings
        {
            public const string Linear = "linear";
            public const string Clerp = "clerp";
            public const string Spring = "spring";

            public const string EaseInSine = "easeinsine";
            public const string EaseOutSine = "easeoutsine";
            public const string EaseInOutSine = "easeinoutsine";

            public const string EaseInCubic = "easeincubic";
            public const string EaseOutCubic = "easeoutcubic";
            public const string EaseInOutCubic = "easeinoutcubic";

            public const string EaseInQuad = "easeinquad";
            public const string EaseOutQuad = "easeoutquad";
            public const string EaseInOutQuad = "easeinoutquad";

            public const string EaseInQuart = "easeinquart";
            public const string EaseOutQuart = "easeoutquart";
            public const string EaseInOutQuart = "easeinoutquart";

            public const string EaseInQuint = "easeinquint";
            public const string EaseOutQuint = "easeoutquint";
            public const string EaseInOutQuint = "easeinoutquint";

            public const string EaseInExpo = "easeinexpo";
            public const string EaseOutExpo = "easeoutexpo";
            public const string EaseInOutExpo = "easeinoutexpo";

            public const string EaseInCirc = "easeincirc";
            public const string EaseOutCirc = "easeoutcirc";
            public const string EaseInOutCirc = "easeinoutcirc";

            public const string EaseInBounce = "easeinbounce";
            public const string EaseOutBounce = "easeoutbounce";
            public const string EaseInOutBounce = "easeinoutbounce";

            public const string EaseOutBack = "easeoutback";
            public const string EaseInOutBack = "easeinoutback";

            public const string EaseInElastic = "easeinelastic";
            public const string EaseOutElastic = "easeoutelastic";
            public const string EaseInOutElastic = "easeinoutelastic";
        }

        private static readonly Dictionary<string, string> EasingNameToDisplayNameDictionary = new()
        {
            { EasingStrings.EaseInSine, "In Sine" },
            { EasingStrings.EaseOutSine, "Out Sine" },
            { EasingStrings.EaseInOutSine, "In Out Sine" },

            { EasingStrings.EaseInCubic, "In Cubic" },
            { EasingStrings.EaseOutCubic, "Out Cubic" },
            { EasingStrings.EaseInOutCubic, "In Out Cubic" },

            { EasingStrings.EaseInQuad, "In Quad" },
            { EasingStrings.EaseOutQuad, "Out Quad" },
            { EasingStrings.EaseInOutQuad, "In Out Quad" },

            { EasingStrings.Linear, "Linear" },
            { EasingStrings.Clerp, "Clerp" },
            { EasingStrings.Spring, "Spring" },

            { EasingStrings.EaseInQuart, "In Quart" },
            { EasingStrings.EaseOutQuart, "Out Quart" },
            { EasingStrings.EaseInOutQuart, "In Out Quart" },

            { EasingStrings.EaseInQuint, "In Quint" },
            { EasingStrings.EaseOutQuint, "Out Quint" },
            { EasingStrings.EaseInOutQuint, "In Out Quint" },

            { EasingStrings.EaseInExpo, "In Expo" },
            { EasingStrings.EaseOutExpo, "Out Expo" },
            { EasingStrings.EaseInOutExpo, "In Out Expo" },

            { EasingStrings.EaseInCirc, "In Circ" },
            { EasingStrings.EaseOutCirc, "Out Circ" },
            { EasingStrings.EaseInOutCirc, "In Out Circ" },

            { EasingStrings.EaseInBounce, "In Bounce" },
            { EasingStrings.EaseOutBounce, "Out Bounce" },
            { EasingStrings.EaseInOutBounce, "In Out Bounce" },

            { EasingStrings.EaseOutBack, "Out Back" },
            { EasingStrings.EaseInOutBack, "In Out Back" },

            { EasingStrings.EaseInElastic, "In Elastic" },
            { EasingStrings.EaseOutElastic, "Out Elastic" },
            { EasingStrings.EaseInOutElastic, "In Out Elastic" }
        };

        public static string GetDisplayName(string easingName)
        {
            return EasingNameToDisplayNameDictionary[easingName];
        }

        public static List<string> UnCommonEasings { get; private set; } = new()
        {
            EasingStrings.Linear,
            EasingStrings.Clerp,
            EasingStrings.Spring,
            EasingStrings.EaseInQuart,
            EasingStrings.EaseOutQuart,
            EasingStrings.EaseInOutQuart,
            EasingStrings.EaseInQuint,
            EasingStrings.EaseOutQuint,
            EasingStrings.EaseInOutQuint,
            EasingStrings.EaseInExpo,
            EasingStrings.EaseOutExpo,
            EasingStrings.EaseInOutExpo,
            EasingStrings.EaseInCirc,
            EasingStrings.EaseOutCirc,
            EasingStrings.EaseInOutCirc,
            EasingStrings.EaseInBounce,
            EasingStrings.EaseOutBounce,
            EasingStrings.EaseInOutBounce,
            EasingStrings.EaseOutBack,
            EasingStrings.EaseInOutBack,
            EasingStrings.EaseInElastic,
            EasingStrings.EaseOutElastic,
            EasingStrings.EaseInOutElastic
        };

        public static Easing GetEasingFunc(string easingName)
        {
            easingName = easingName.ToLower();

            return easingName switch
            {
                EasingStrings.Linear => Linear,
                EasingStrings.Clerp => Clerp,
                EasingStrings.Spring => Spring,
                EasingStrings.EaseInQuad => EaseInQuad,
                EasingStrings.EaseOutQuad => EaseOutQuad,
                EasingStrings.EaseInOutQuad => EaseInOutQuad,
                EasingStrings.EaseInCubic => EaseInCubic,
                EasingStrings.EaseOutCubic => EaseOutCubic,
                EasingStrings.EaseInOutCubic => EaseInOutCubic,
                EasingStrings.EaseInQuart => EaseInQuart,
                EasingStrings.EaseOutQuart => EaseOutQuart,
                EasingStrings.EaseInOutQuart => EaseInOutQuart,
                EasingStrings.EaseInQuint => EaseInQuint,
                EasingStrings.EaseOutQuint => EaseOutQuint,
                EasingStrings.EaseInOutQuint => EaseInOutQuint,
                EasingStrings.EaseInSine => EaseInSine,
                EasingStrings.EaseOutSine => EaseOutSine,
                EasingStrings.EaseInOutSine => EaseInOutSine,
                EasingStrings.EaseInExpo => EaseInExpo,
                EasingStrings.EaseOutExpo => EaseOutExpo,
                EasingStrings.EaseInOutExpo => EaseInOutExpo,
                EasingStrings.EaseInCirc => EaseInCirc,
                EasingStrings.EaseOutCirc => EaseOutCirc,
                EasingStrings.EaseInOutCirc => EaseInOutCirc,
                EasingStrings.EaseInBounce => EaseInBounce,
                EasingStrings.EaseOutBounce => EaseOutBounce,
                EasingStrings.EaseInOutBounce => EaseInOutBounce,
                EasingStrings.EaseOutBack => EaseOutBack,
                EasingStrings.EaseInOutBack => EaseInOutBack,
                EasingStrings.EaseInElastic => EaseInElastic,
                EasingStrings.EaseOutElastic => EaseOutElastic,
                EasingStrings.EaseInOutElastic => EaseInOutElastic,
                _ => throw new Exception("Not a valid Easing Type: " + easingName),
            };
        }
        
        public static float BlendEasing(
            float start,
            float end,
            float normalizedProgress,
            float blendStartPercentage,
            Func<float, float, float, float> ease1,
            Func<float, float, float, float> ease2)
        {
            if (normalizedProgress <= blendStartPercentage)
            {
                return ease1(start, end, normalizedProgress);
            }
            
            float localT = Mathf.InverseLerp(blendStartPercentage, 1f, normalizedProgress);
            float eased = ease2(0, 1, localT);
            float offset = ease1(start, end, blendStartPercentage); // Use actual start/end
            float remaining = end - offset;
            return offset + remaining * eased;
        }
        
        public static float BlendEasing2(
            float start,
            float end,
            float normalizedProgress,
            float blendStartPercentage,
            Func<float, float, float, float> ease1,
            Func<float, float, float, float> ease2)
        {
            // Clamp progress to 0-1
            normalizedProgress = Mathf.Clamp01(normalizedProgress);
    
            if (normalizedProgress < blendStartPercentage)
            {
                // Use ease1 for the first portion
                return ease1(start, end, normalizedProgress / blendStartPercentage);
            }
    
            // Calculate derivative (velocity) of ease1 at the blend point
            float epsilon = 0.001f;
            float ease1AtBlend = ease1(start, end, 1.0f);
            float ease1BeforeBlend = ease1(start, end, Mathf.Max(0, 1.0f - epsilon));
            float ease1Velocity = (ease1AtBlend - ease1BeforeBlend) / epsilon;
    
            // Calculate what ease2 would give us for the second portion
            float local = (normalizedProgress - blendStartPercentage) / (1.0f - blendStartPercentage);
            float ease2Result = ease2(ease1AtBlend, end, local);
    
            // Calculate derivative of ease2 at the start of the blend zone
            float ease2AtStart = ease2(ease1AtBlend, end, 0.0f);
            float ease2AfterStart = ease2(ease1AtBlend, end, epsilon);
            float ease2Velocity = (ease2AfterStart - ease2AtStart) / epsilon;
    
            // Blend factor based on how different the velocities are
            // If velocities match, use pure ease2; if they don't, we need to adjust
            float velocityRatio = ease1Velocity / (ease2Velocity + 0.0001f); // Avoid division by zero
    
            // Apply velocity correction
            // Use a custom curve that starts with ease1's velocity
            float correctedLocal = local - 0.5f * (1.0f - velocityRatio) * local * local;
            return ease2(ease1AtBlend, end, correctedLocal);
        }

        // Solution 3: Hermite Spline Blend
        public static float BlendEasingHermite(
            float start,
            float end,
            float normalizedProgress,
            float blendStartPercentage,
            Func<float, float, float, float> ease1,
            Func<float, float, float, float> ease2)
        {
            // Clamp progress to 0-1
            normalizedProgress = Mathf.Clamp01(normalizedProgress);
            
            if (normalizedProgress < blendStartPercentage)
            {
                // Use ease1 for the first portion
                return ease1(start, end, normalizedProgress / blendStartPercentage);
            }
            
            // Calculate position and velocity at blend point for ease1
            float epsilon = 0.001f;
            float p0 = ease1(start, end, 1.0f); // Position at blend start
            float ease1Before = ease1(start, end, Mathf.Max(0, 1.0f - epsilon));
            float m0 = (p0 - ease1Before) / epsilon; // Velocity at blend start
            
            // Calculate position and velocity at end point for ease2
            float p1 = end; // End position
            float ease2AtEnd = ease2(start, end, 1.0f);
            float ease2Before = ease2(start, end, Mathf.Max(0, 1.0f - epsilon));
            float m1 = (ease2AtEnd - ease2Before) / epsilon; // Velocity at end
            
            // Remap to 0-1 in the blend zone
            float t = (normalizedProgress - blendStartPercentage) / (1.0f - blendStartPercentage);
            
            // Cubic Hermite spline
            float t2 = t * t;
            float t3 = t2 * t;
            
            float h00 = 2 * t3 - 3 * t2 + 1;
            float h10 = t3 - 2 * t2 + t;
            float h01 = -2 * t3 + 3 * t2;
            float h11 = t3 - t2;
            
            // Scale tangents by the interval length
            float intervalLength = 1.0f - blendStartPercentage;
            
            return h00 * p0 + h10 * m0 * intervalLength + h01 * p1 + h11 * m1 * intervalLength;
        }

        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        public static float Clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            return retval;
        }

        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value + start;
            value--;
            return -end / 2 * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value + start;
            value -= 2;
            return -end / 2 * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        /* GFX47 MOD START */
        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float bounce(float start, float end, float value){
        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d / 2) return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }
        /* GFX47 MOD END */

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value / 1) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525f);
            return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float Punch(float amplitude, float value)
        {
            float s = 9;
            if (value == 0)
            {
                return 0;
            }
            if (value == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        /* GFX47 MOD START */
        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float elastic(float start, float end, float value){
        public static float EaseOutElastic(float start, float end, float value)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /* GFX47 MOD START */
        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d / 2) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
        /* GFX47 MOD END */
    }

}