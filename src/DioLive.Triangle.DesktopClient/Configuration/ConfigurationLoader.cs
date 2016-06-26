using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace DioLive.Triangle.DesktopClient.Configuration
{
    public static class ConfigurationLoader
    {
        public static IConfiguration Load(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            ConfigurationAdapter configuration = JsonConvert.DeserializeObject<ConfigurationAdapter>(fileContent);

            string serverUri = configuration.ServerUri;
            if (!serverUri.EndsWith("/"))
            {
                serverUri += "/";
            }

            if (!serverUri.StartsWith("http://") && !serverUri.StartsWith("https://"))
            {
                serverUri = "http://" + serverUri;
            }

            Color background = XnaHelpers.ParseColor(configuration.Colors.Background);
            Color[] teams = configuration.Colors.Teams.Select(XnaHelpers.ParseColor).ToArray();
            Color beamColor = XnaHelpers.ParseColor(configuration.Colors.Beam);

            ColorsSection colors = new ColorsSection(background, teams, beamColor);

            return new DefaultConfiguration(colors, serverUri);
        }
    }
}