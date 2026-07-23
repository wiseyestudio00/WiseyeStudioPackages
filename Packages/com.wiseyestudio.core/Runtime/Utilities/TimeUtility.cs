using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio
{
    public static class TimeUtility
    {
        public static float MillieSecondToSecond(int millisecond)
        {
            return millisecond / 1000f;
        }

        public static double MilliSecondToSecondAsDouble(int millisecond)
        {
            return millisecond / 1000d;
        }

        public static int SecondToMillieSecond(double second)
        {
            bool isPositive = second > 0;
            var res = (int)(second * 1000);

            if (isPositive)
            {
                if (res < 0)
                {
                    res = int.MaxValue;
                }
            }

            return res;
        }

        public static int SecondToMillieSecond(float second)
        {
            bool isPositive = second > 0;
            var res = (int)(second * 1000);

            if (isPositive)
            {
                if (res < 0)
                {
                    res = int.MaxValue;
                }
            }

            return res;
        }

        public static long SecondToLongMillieSecond(double second)
        {
            bool isPositive = second > 0;

            var res = (long)(second * 1000);

            if (isPositive)
            {
                if (res < 0)
                {
                    res = long.MaxValue;
                }
            }

            return res;
        }
    }
}
