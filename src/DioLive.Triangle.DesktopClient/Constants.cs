using System;

namespace DioLive.Triangle.DesktopClient
{
    public static class Constants
    {
        public static class UI
        {
            public const int DotRadius = 250;
            public const int NeighbourhoodSize = 500;
            public const int RadarSize = 100;
            public const int RadarDotRadius = 2;
            public const int BeamLength = 2000;
            public const int BeamWidth = 75;
        }

        public static class Space
        {
            public const int Scope = 5000;
            public const int RadarScope = 50000;
        }

        public static class Timers
        {
            public static TimeSpan SendUpdateInterval { get; } = TimeSpan.FromMilliseconds(50);

            public static TimeSpan GetCurrentInterval { get; } = TimeSpan.FromMilliseconds(50);

            public static TimeSpan GetNeighboursInterval { get; } = TimeSpan.FromMilliseconds(200);

            public static TimeSpan GetRadarInterval { get; } = TimeSpan.FromMilliseconds(500);
        }
    }
}