using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class CurrentDot
    {
        static CurrentDot()
        {
            Destroyed = new CurrentDot(DotState.Destroyed, default(float), default(float?));
        }

        [JsonConstructor]
        public CurrentDot(DotState state, float moveDirection, float? beaming)
        {
            this.State = state;
            this.MoveDirection = moveDirection;
            this.Beaming = beaming;
        }

        public static CurrentDot Destroyed { get; }

        public DotState State { get; }

        public float? Beaming { get; }

        public float MoveDirection { get; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}