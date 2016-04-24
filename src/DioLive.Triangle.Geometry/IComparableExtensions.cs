namespace System
{
    public static class IComparableExtensions
    {
        public static bool Between<T>(this T number, T bound1, T bound2)
            where T : IComparable<T>
        {
            if (bound1.CompareTo(bound2) < 0)
            {
                return number.CompareTo(bound1) >= 0 && number.CompareTo(bound2) <= 0;
            }
            else
            {
                return number.CompareTo(bound2) >= 0 && number.CompareTo(bound1) <= 0;
            }
        }
    }
}