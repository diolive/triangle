using System;
using System.Collections.Generic;
using System.Linq;
using DioLive.Triangle.Geometry;

namespace DioLive.Triangle.DataStorage
{
    public class Space
    {
        public const float InitVelocity = 100f;

        private const int NeighbourhoodSize = 500;
        private const int RadarSize = 5000;
        private const int BeamLength = 250;
        private const int DotRadius = 25;
        private const float Deceleration = 10f;

        private readonly ICollection<Dot> dots;

        public Space()
        {
            this.dots = new HashSet<Dot>();
        }

        public void Add(Dot dot)
        {
            this.dots.Add(dot);
        }

        public Dot FindById(Guid id)
        {
            return this.dots.FirstOrDefault(dot => dot.Id == id);
        }

        public void RemoveById(Guid id)
        {
            var dot = FindById(id);
            if (dot != null)
            {
                dots.Remove(dot);
            }
        }

        public IEnumerable<Dot> GetNeighbours(int x, int y)
        {
            var scope = new Rectangle(x, y, NeighbourhoodSize, NeighbourhoodSize);
            return this.dots.Where(dot => scope.Contains(dot.X, dot.Y));
        }

        public IEnumerable<Dot> GetRadar(byte team, int x, int y)
        {
            var scope = new Rectangle(x, y, RadarSize, RadarSize);
            return this.dots.Where(dot => (dot.Team == team || dot.State != DotState.Free) && scope.Contains(dot.X, dot.Y));
        }

        public void Update(TimeSpan elapsedTime)
        {
            double time = elapsedTime.TotalSeconds;
            foreach (var dot in this.dots.Where(dot => dot.State != DotState.Stunned && dot.Velocity > 0))
            {
                double move = Math.Max(0, dot.Velocity * time - (Deceleration * time * time) / 2);
                if (move > 0)
                {
                    dot.X += (float)(Math.Cos(dot.MoveDirection) * move);
                    dot.Y += (float)(Math.Sin(dot.MoveDirection) * move);
                    dot.Velocity = Math.Max(0f, (float)(dot.Velocity - Deceleration * time));
                }
                else
                {
                    dot.Velocity = 0f;
                }
            }

            List<Dot> firedDots = new List<Dot>();
            foreach (var dot in this.dots.Where(d => d.Beaming.HasValue))
            {
                Beam beam = new Beam(dot.X, dot.Y, dot.Beaming.Value, BeamLength);
                firedDots.AddRange(this.dots.Except(new[] { dot }).Where(d => beam.CrossedWithCircle(new Circle(d.X, d.Y, DotRadius))));
            }

            ILookup<Dot, Dot> firedDotsLookup = firedDots.ToLookup(dot => dot);

            List<Dot> destroyedDots = new List<Dot>();
            foreach (var dot in this.dots)
            {
                int beamCount = firedDotsLookup[dot].Count();

                if (beamCount > 2)
                {
                    destroyedDots.Add(dot);
                }
                else
                {
                    dot.State = (DotState)beamCount;
                }

                dot.Beaming = null;
            }

            foreach (var dot in destroyedDots)
            {
                this.dots.Remove(dot);
            }
        }
    }
}