using System;
using Microsoft.Xna.Framework;

namespace DioLive.Triangle.DesktopClient
{
    public static class XnaHelpers
    {
        public static Color ParseColor(string hexColor)
        {
            switch (hexColor.Length)
            {
                case 8:
                    return ParseAARRGGBB(hexColor);

                case 6:
                    return ParseRRGGBB(hexColor);

                case 3:
                    return ParseRGB(hexColor);

                default:
                    throw new ArgumentException($"Unknown color definition: {hexColor}", nameof(hexColor));
            }
        }

        private static Color ParseRGB(string hexColor)
        {
            byte r = Convert.ToByte(new string(hexColor[0], 2), 16);
            byte g = Convert.ToByte(new string(hexColor[1], 2), 16);
            byte b = Convert.ToByte(new string(hexColor[2], 2), 16);
            return new Color(r, g, b, 255);
        }

        private static Color ParseRRGGBB(string hexColor)
        {
            byte r = Convert.ToByte(hexColor.Substring(0, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(2, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(4, 2), 16);
            return new Color(r, g, b, 255);
        }

        private static Color ParseAARRGGBB(string hexColor)
        {
            byte a = Convert.ToByte(hexColor.Substring(0, 2), 16);
            byte r = Convert.ToByte(hexColor.Substring(2, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(4, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(6, 2), 16);
            return new Color(r, g, b, a);
        }
    }
}