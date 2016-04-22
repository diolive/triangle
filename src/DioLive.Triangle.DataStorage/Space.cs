using System;
using System.Collections.Generic;
using System.Linq;
using DioLive.Triangle.DataStorage.Geometry;

namespace DioLive.Triangle.DataStorage
{
    public class Space
    {
        private const int NeighbourhoodWidth = 500;
        private const int NeighbourhoodHeight = 500;
        private const int BeamLength = 250;
        private const int RadarWidth = 5000;
        private const int RadarHeight = 5000;
        private const int DotRadius = 25;
        private const float DotVelocity = 0.25f;

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
            var scope = new Rectangle(x, y, NeighbourhoodWidth, NeighbourhoodHeight);
            return this.dots.Where(dot => scope.Contains(dot.X, dot.Y));
        }

        public IEnumerable<Dot> GetRadar(byte team, int x, int y)
        {
            var scope = new Rectangle(x, y, RadarWidth, RadarHeight);
            return this.dots.Where(dot => (dot.Team == team || dot.State != DotState.Free) && scope.Contains(dot.X, dot.Y));
        }

        public void Update(TimeSpan elapsedTime)
        {
            foreach (var dot in this.dots.Where(dot => dot.State != DotState.Stunned))
            {
                dot.X += (float)(Math.Cos(dot.MoveDirection) * elapsedTime.TotalMilliseconds * DotVelocity);
                dot.Y += (float)(Math.Sin(dot.MoveDirection) * elapsedTime.TotalMilliseconds * DotVelocity);
            }

            List<Dot> firedDots = new List<Dot>();
            foreach (var dot in this.dots.Where(d => d.Beaming.HasValue))
            {
                Beam beam = new Beam(dot.X, dot.Y, dot.Beaming.Value, BeamLength);
                firedDots.AddRange(this.dots.Except(new[] { dot }).Where(d => beam.CrossedWithCircle(new Circle(d.X, d.Y, DotRadius))));
            }

            ILookup<Dot,Dot> firedDotsLookup = firedDots.ToLookup(dot => dot);

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
            }

            foreach (var dot in destroyedDots)
            {
                this.dots.Remove(dot);
            }
        }
    }
}