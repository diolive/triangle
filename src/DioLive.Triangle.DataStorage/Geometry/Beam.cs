using System;

namespace DioLive.Triangle.DataStorage.Geometry
{
    public struct Beam
    {
        public Beam(float beginX, float beginY, float direction, float length)
        {
            this.BeginX = beginX;
            this.BeginY = beginY;

            this.EndX = (float)(beginX + Math.Cos(direction) * length);
            this.EndY = (float)(beginY + Math.Sin(direction) * length);
        }

        public float BeginX { get; }

        public float BeginY { get; }

        public float EndX { get; }

        public float EndY { get; }

        public bool CrossedWithCircle(Circle circle)
        {
            //Проверка на нахождение одного из концов отрезка в круге
            if (circle.Contains(this.BeginX, this.BeginY) || circle.Contains(this.EndX, this.EndY))
            {
                return true;
            }

            //axis-aligned
            if (this.BeginX == this.EndX)
            {
                return circle.Y.Between(this.BeginY, this.EndY) && Math.Abs(this.BeginX - circle.X) <= circle.Radius;
            }

            if (this.BeginY == this.EndY)
            {
                return circle.X.Between(this.BeginX, this.EndX) && Math.Abs(this.BeginY - circle.Y) <= circle.Radius;
            }

            //Находим точку (px,py) пересечения перпендикуляра от центра круга к линии,
            //которой принадлежит отрезок.
            float a = (this.BeginY - this.EndY) / (this.BeginX - this.EndX);
            float b = this.BeginY - a * this.BeginX;
            float px = (circle.Y - b + circle.X / a) / (a + 1 / a);
            float py = a * px + b;

            //Принадлежит отрезку?
            return px.Between(this.BeginX, this.EndX) && circle.Contains(px, py);
        }
    }
}