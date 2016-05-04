namespace DioLive.Common.Helpers
{
    public static class MotionHelper
    {
        public static double GetShift(double velocity, double time)
        {
            return velocity * time;
        }

        public static double GetShift(double initVelocity, double acceleration, double time, bool canBeNegative = true)
        {
            double shift = (initVelocity + (acceleration * time / 2)) * time;
            if (!canBeNegative && shift < 0)
            {
                shift = 0;
            }

            return shift;
        }

        public static double GetVelocity(double initVelocity, double acceleration, double time, bool canBeNegative = true)
        {
            double velocity = initVelocity + (acceleration * time);
            if (!canBeNegative && velocity < 0)
            {
                velocity = 0;
            }

            return velocity;
        }

        public static double GetShift(ref double velocity, double acceleration, double time, bool canBeNegative = true)
        {
            double shift = GetShift(velocity, acceleration, time, canBeNegative);
            velocity = GetVelocity(velocity, acceleration, time, canBeNegative);
            return shift;
        }
    }
}