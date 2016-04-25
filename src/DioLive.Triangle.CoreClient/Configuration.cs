namespace DioLive.Triangle.CoreClient
{
    internal class Configuration
    {
        public Colors Colors { get; set; }

        public string ServerUri { get; set; }
    }

    internal class Colors
    {
        public string Background { get; set; }

        public string[] Teams { get; set; }

        public string Beam { get; set; }
    }
}