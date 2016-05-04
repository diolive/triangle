using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class RadarResponse
    {
        static RadarResponse()
        {
            None = new RadarResponse(new RadarDot[0]);
        }

        public static RadarResponse None { get; }

        [JsonConstructor]
        public RadarResponse(RadarDot[] radarDots)
        {
            this.RadarDots = radarDots;
        }

        public RadarDot[] RadarDots { get; }
    }
}