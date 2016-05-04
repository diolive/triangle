using System;
using Microsoft.Xna.Framework;

namespace DioLive.Triangle.CoreClient
{
    public static class XnaHelpers
    {
        public static Color ParseColor(string hexColor)
        {
            byte r;
            byte g;
            byte b;
            byte a;

            switch (hexColor.Length)
            {
                case 8:
                    a = Convert.ToByte(hexColor.Substring(0, 2), 16);
                    r = Convert.ToByte(hexColor.Substring(2, 2), 16);
                    g = Convert.ToByte(hexColor.Substring(4, 2), 16);
                    b = Convert.ToByte(hexColor.Substring(6, 2), 16);
                    break;

                case 6:
                    a = 255;
                    r = Convert.ToByte(hexColor.Substring(0, 2), 16);
                    g = Convert.ToByte(hexColor.Substring(2, 2), 16);
                    b = Convert.ToByte(hexColor.Substring(4, 2), 16);
                    break;

                case 3:
                    a = 255;
                    r = Convert.ToByte(new string(hexColor[0], 2), 16);
                    g = Convert.ToByte(new string(hexColor[1], 2), 16);
                    b = Convert.ToByte(new string(hexColor[2], 2), 16);
                    break;

                default:
                    throw new ArgumentException($"Unknown color definition: {hexColor}", nameof(hexColor));
            }

            return new Color(r, g, b, a);
        }
    }
}