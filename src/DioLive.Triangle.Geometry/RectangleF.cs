using System;

namespace DioLive.Triangle.Geometry
{
    public struct RectangleF
    {
        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            this.minX = x - (width / 2);
            this.maxX = x + (width / 2);
            this.minY = y - (height / 2);
            this.maxY = y + (height / 2);
        }

        public float X { get; }

        public float Y { get; }

        public float Width { get; }

        public float Height { get; }

        public bool Contains(float x, float y)
        {
            return x.Between(this.minX, this.maxX) && y.Between(this.minY, this.maxY);
        }
    }
}