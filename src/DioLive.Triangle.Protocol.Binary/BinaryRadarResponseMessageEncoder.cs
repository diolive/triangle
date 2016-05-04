using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinaryRadarResponseMessageEncoder : BinaryMessageEncoder<RadarResponse>
    {
        public override RadarResponse Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                byte count = body.ReadByte();
                RadarDot[] radarDots = new RadarDot[count];
                for (int i = 0; i < count; i++)
                {
                    byte team = body.ReadByte();
                    byte rx = body.ReadByte();
                    byte ry = body.ReadByte();
                    radarDots[i] = new RadarDot(team, rx, ry);
                }

                return new RadarResponse(radarDots);
            }
        }

        public override void Write(Stream stream, RadarResponse request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteByte((byte)request.RadarDots.Length);
                foreach (var radar in request.RadarDots)
                {
                    body.WriteByte(radar.Team);
                    body.WriteByte(radar.RX);
                    body.WriteByte(radar.RY);
                }
            }
        }
    }
}