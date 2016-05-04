using System;

namespace DioLive.Triangle.Geometry
{
    public struct Rectangle
    {
        public Rectangle(int centerX, int centerY, int width, int height)
        {
            this.CenterX = centerX;
            this.CenterY = centerY;
            this.Width = width;
            this.Height = height;

            this.Left = centerX - (width / 2);
            this.Right = centerX + (width / 2);
            this.Top = centerY - (height / 2);
            this.Bottom = centerY + (height / 2);
        }

        public int CenterX { get; }

        public int CenterY { get; }

        public int Width { get; }

        public int Height { get; }

        public int Left { get; }

        public int Right { get; }

        public int Top { get; }

        public int Bottom { get; }

        public bool Contains(int x, int y)
        {
            return x.Between(this.Left, this.Right) && y.Between(this.Top, this.Bottom);
        }
    }
}