namespace DioLive.Triangle.CoreClient.Configuration
{
    public interface IConfiguration
    {
        ColorsSection Colors { get; }

        string ServerUri { get; }
    }
}