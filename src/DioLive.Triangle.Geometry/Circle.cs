namespace DioLive.Triangle.Geometry
{
    public struct Circle
    {
        private float squaredRadius;

        public Circle(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;

            this.squaredRadius = radius * radius;
        }

        public float X { get; }

        public float Y { get; }

        public float Radius { get; }

        public bool Contains(float x, float y)
        {
            float dx = x - this.X;
            float dy = y - this.Y;

            return (dx * dx) + (dy * dy) <= squaredRadius;
        }
    }
}