namespace DioLive.Triangle.DesktopClient.Configuration
{
    public interface IConfiguration
    {
        ColorsSection Colors { get; }

        string ServerUri { get; }
    }
}