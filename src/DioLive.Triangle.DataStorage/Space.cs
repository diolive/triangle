using System;
using System.Collections.Generic;
using System.Linq;

using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.Geometry;

namespace DioLive.Triangle.DataStorage
{
    public class Space
    {
        public const float InitVelocity = 1000f;

        private const int NeighbourhoodSize = 5000;
        private const double NeighbourhoodCoef = (double)ushort.MaxValue / NeighbourhoodSize;
        private const int RadarSize = 50000;
        private const double RadarCoef = (double)byte.MaxValue / RadarSize;
        private const int BeamLength = 2000;
        private const int DotRadius = 250;
        private const float Deceleration = 500f;

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
                this.dots.Remove(dot);
            }
        }

        public IEnumerable<NeighbourDot> GetNeighbours(int x, int y)
        {
            var scope = new Rectangle(x, y, NeighbourhoodSize, NeighbourhoodSize);
            return this.dots
                .Where(dot => scope.Contains(dot.X, dot.Y))
                .Select(dot => new NeighbourDot(dot.Team, (ushort)((dot.X - scope.Left) * NeighbourhoodCoef), (ushort)((dot.Y - scope.Top) * NeighbourhoodCoef), dot.State, dot.BeamDirection));
        }

        public IEnumerable<RadarDot> GetRadar(byte team, int x, int y)
        {
            var scope = new Rectangle(x, y, RadarSize, RadarSize);
            return this.dots
                .Where(dot => (dot.Team == team || dot.State.HasFlag(DotState.Fired)) && scope.Contains(dot.X, dot.Y))
                .Select(dot => new RadarDot(dot.Team, (byte)((dot.X - scope.Left) * RadarCoef), (byte)((dot.Y - scope.Top) * RadarCoef)));
        }

        public void Update(TimeSpan elapsedTime)
        {
            double time = elapsedTime.TotalSeconds;
            foreach (var dot in this.dots.Where(dot => !dot.State.HasFlag(DotState.Stunned) && dot.Velocity > 0))
            {
                double move = MotionHelper.GetShift(dot.Velocity, -Deceleration, time, canBeNegative: false);
                double angle = AngleHelper.DirectionToRadians(dot.MoveDirection);
                if (move > 0)
                {
                    dot.X += (int)(Math.Cos(angle) * move);
                    dot.Y += (int)(Math.Sin(angle) * move);
                    dot.Velocity = (float)MotionHelper.GetVelocity(dot.Velocity, -Deceleration, time, canBeNegative: false);
                }
                else
                {
                    dot.Velocity = 0f;
                }
            }

            List<Dot> firedDots = new List<Dot>();
            foreach (var dot in this.dots.Where(d => d.State.HasFlag(DotState.Beaming)))
            {
                Beam beam = new Beam(dot.X, dot.Y, (float)AngleHelper.DirectionToRadians(dot.BeamDirection), BeamLength);
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
                    DotState newState = dot.State & (DotState.Alive | DotState.Beaming);
                    if (beamCount >= 1)
                    {
                        newState |= DotState.Fired;
                    }

                    if (beamCount >= 2)
                    {
                        newState |= DotState.Stunned;
                    }

                    dot.State = newState;
                }
            }

            foreach (var dot in destroyedDots)
            {
                dot.State = DotState.Destroyed;
                this.dots.Remove(dot);
            }
        }

        public IEnumerable<Dot> GetAllDots() => this.dots.ToList();
    }
}