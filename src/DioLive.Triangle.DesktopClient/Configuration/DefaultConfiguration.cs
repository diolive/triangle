namespace DioLive.Triangle.DesktopClient.Configuration
{
    public class DefaultConfiguration : IConfiguration
    {
        public DefaultConfiguration(ColorsSection colors, string serverUri)
        {
            this.Colors = colors;
            this.ServerUri = serverUri;
        }

        public ColorsSection Colors { get; }

        public string ServerUri { get; }
    }
}