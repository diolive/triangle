using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace DioLive.Triangle.DesktopClient.Configuration
{
    public static class ConfigurationLoader
    {
        public static IConfiguration Load(params string[] fileNames)
        {
            string serverUri = null;
            string backgroundColor = null;
            string[] teamsColors = null;
            string beamColor = null;

            foreach (var fileName in fileNames)
            {
                string fileContent = File.ReadAllText(fileName);
                ConfigurationAdapter configuration = JsonConvert.DeserializeObject<ConfigurationAdapter>(fileContent);

                if (configuration.ServerUri != null)
                {
                    serverUri = configuration.ServerUri;
                }

                if (configuration.Colors != null)
                {
                    if (configuration.Colors.Background != null)
                    {
                        backgroundColor = configuration.Colors.Background;
                    }

                    if (configuration.Colors.Teams != null)
                    {
                        teamsColors = configuration.Colors.Teams;
                    }

                    if (configuration.Colors.Beam != null)
                    {
                        beamColor = configuration.Colors.Beam;
                    }
                }
            }

            if (serverUri == null)
            {
                throw new ConfigurationException("Property serverUri was not defined");
            }

            if (backgroundColor == null)
            {
                throw new ConfigurationException("Property colors:background was not defined");
            }

            if (teamsColors == null)
            {
                throw new ConfigurationException("Property colors:team was not defined");
            }

            if (beamColor == null)
            {
                throw new ConfigurationException("Property colors:beam was not defined");
            }

            if (!serverUri.EndsWith("/"))
            {
                serverUri += "/";
            }

            if (!serverUri.StartsWith("http://") && !serverUri.StartsWith("https://"))
            {
                serverUri = "http://" + serverUri;
            }

            Color background = XnaHelpers.ParseColor(backgroundColor);
            Color[] teams = teamsColors.Select(XnaHelpers.ParseColor).ToArray();
            Color beam = XnaHelpers.ParseColor(beamColor);

            ColorsSection colors = new ColorsSection(background, teams, beam);

            return new DefaultConfiguration(colors, serverUri);
        }
    }
}