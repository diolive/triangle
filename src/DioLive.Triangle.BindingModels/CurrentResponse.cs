using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class CurrentResponse
    {
        static CurrentResponse()
        {
            Destroyed = new CurrentResponse(DotState.Destroyed);
        }

        [JsonConstructor]
        public CurrentResponse(DotState state, byte moveDirection, byte beamDirection = default(byte))
        {
            this.State = state;
            this.MoveDirection = moveDirection;
            this.BeamDirection = beamDirection;
        }

        private CurrentResponse(DotState state)
        {
            this.State = state;
        }

        public static CurrentResponse Destroyed { get; }

        public DotState State { get; }

        public byte BeamDirection { get; }

        public byte MoveDirection { get; }
    }
}