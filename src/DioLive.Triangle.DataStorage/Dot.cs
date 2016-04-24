using System;

namespace DioLive.Triangle.DataStorage
{
    public class Dot
    {
        public Dot(byte team, float x, float y)
        {
            this.Id = Guid.NewGuid();
            this.Team = team;
            this.X = x;
            this.Y = y;
            this.State = DotState.Free;
        }

        public Guid Id { get; }

        public byte Team { get; }

        public float X { get; set; }

        public float Y { get; set; }

        public float MoveDirection { get; set; }

        public float Velocity { get; set; }

        public float? Beaming { get; set; }

        public DotState State { get; set; }
    }
}