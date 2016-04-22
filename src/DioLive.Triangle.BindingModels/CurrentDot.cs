using DioLive.Triangle.DataStorage;

namespace DioLive.Triangle.BindingModels
{
    public class CurrentDot
    {
        public CurrentDot(DotState state, float moveDirection, float? beaming)
            : this(true, state, moveDirection, beaming)
        {
        }

        private CurrentDot(bool alive, DotState state, float moveDirection, float? beaming)
        {
            this.Alive = alive;
            this.State = state;
            this.MoveDirection = moveDirection;
            this.Beaming = beaming;
        }

        public static CurrentDot Destroyed => new CurrentDot(false, default(DotState), default(float), default(float?));

        public bool Alive { get; }

        public DotState State { get; }

        public float? Beaming { get; }

        public float MoveDirection { get; }
    }
}