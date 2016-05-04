using System;

namespace DioLive.Common.Helpers
{
    public static class AngleHelper
    {
        private const double Tau = Math.PI * 2;
        private const double ByteToRadiansCoef = Tau / 256;

        public static double DirectionToRadians(byte direction)
        {
            return direction * ByteToRadiansCoef;
        }

        public static byte RadiansToDirection(double radians)
        {
            radians = ((radians % Tau) + Tau) % Tau;
            return (byte)(radians / ByteToRadiansCoef);
        }
    }
}