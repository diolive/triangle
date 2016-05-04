using System;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.DataStorage
{
    public class Dot
    {
        public Dot(byte team, int x, int y)
        {
            this.Id = Guid.NewGuid();
            this.Team = team;
            this.X = x;
            this.Y = y;
            this.State = DotState.Alive;
        }

        public Guid Id { get; }

        public byte Team { get; }

        public DotState State { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public float Velocity { get; set; }

        public byte MoveDirection { get; set; }

        public byte BeamDirection { get; set; }
    }
}