using System.Globalization;

namespace WiseyeStudio
{
    public static class FloatUtility
    {
        public static bool TryCultureInvariantParse(string toBeParsed, out float result)
        {
            return float.TryParse(toBeParsed, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static float CultureInvariantParse(string toBeParsed)
        {
            return float.Parse(toBeParsed, CultureInfo.InvariantCulture);
        }

        public static string ToCulturalInvariantString(float toBeConverted, string format = null)
        {
            if (format != null)
            {
                return toBeConverted.ToString(format, CultureInfo.InvariantCulture);
            }
            
            return toBeConverted.ToString(CultureInfo.InvariantCulture);
        }
    }
}