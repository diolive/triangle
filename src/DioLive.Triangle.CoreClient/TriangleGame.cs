using System;
using System.IO;
using System.Linq;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.CoreClient.Configuration;
using DioLive.Triangle.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace DioLive.Triangle.CoreClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TriangleGame : Game
    {
        private readonly int windowWidth;
        private readonly int windowHeight;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Color background;
        private Color[] teamColors;
        private Texture2D dotTexture;
        private Texture2D beamTexture;
        private Rectangle neighbourhoodRect;
        private Rectangle radarRect;
        private IConfiguration configuration;

        private Rectangle windowBounds;
        private Vector2 dotSize;
        private Vector2 beamSize;
        private Vector2 radarDotSize;
        private Color beamColor;

        private ServerClientBase client;

        private CurrentResponse current;
        private NeighboursResponse neighbours;
        private RadarResponse radar;

        private GameTimer sendUpdateTimer;
        private GameTimer getCurrentTimer;
        private GameTimer getNeighboursTimer;
        private GameTimer getRadarTimer;

        public TriangleGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Environment.CurrentDirectory, "Content");

            this.windowWidth = Constants.UI.NeighbourhoodSize + (2 * (int)((float)Constants.UI.DotRadius * Constants.UI.NeighbourhoodSize / Constants.Space.Scope)) + Constants.UI.RadarSize;
            this.windowHeight = Constants.UI.NeighbourhoodSize + (2 * (int)((float)Constants.UI.DotRadius * Constants.UI.NeighbourhoodSize / Constants.Space.Scope));

            graphics.PreferredBackBufferWidth = this.windowWidth;
            graphics.PreferredBackBufferHeight = this.windowHeight;

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var viewport = GraphicsDevice.Viewport;
            this.neighbourhoodRect = new Rectangle(0, 0, Constants.UI.NeighbourhoodSize, Constants.UI.NeighbourhoodSize);
            this.radarRect = new Rectangle(Constants.UI.NeighbourhoodSize, 0, Constants.UI.RadarSize, Constants.UI.RadarSize);
            this.windowBounds = new Rectangle(0, 0, this.windowWidth, this.windowHeight);
            this.dotSize = new Vector2(Constants.UI.DotRadius * 2) * Constants.UI.NeighbourhoodSize / Constants.Space.Scope;
            this.beamSize = new Vector2(Constants.UI.BeamLength, Constants.UI.BeamWidth) * Constants.UI.NeighbourhoodSize / Constants.Space.Scope;
            this.radarDotSize = new Vector2(Constants.UI.RadarDotRadius);

            this.sendUpdateTimer = new GameTimer(Constants.Timers.SendUpdateInterval);
            this.getCurrentTimer = new GameTimer(Constants.Timers.GetCurrentInterval);
            this.getNeighboursTimer = new GameTimer(Constants.Timers.GetNeighboursInterval);
            this.getRadarTimer = new GameTimer(Constants.Timers.GetRadarInterval);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.configuration = ConfigurationLoader.Load("appconfig.json");
            this.client = new BinaryServerClient(new Uri(this.configuration.ServerUri));

            string serverUri = configuration.ServerUri;
            if (!serverUri.EndsWith("/"))
            {
                serverUri += "/";
            }

            if (!serverUri.StartsWith("http://") && !serverUri.StartsWith("https://"))
            {
                serverUri = "http://" + serverUri;
            }
            Assets.Load(this.Content);

            this.client = new BinaryServerClient(new Uri(serverUri));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (this.current != null && !this.current.State.HasFlag(DotState.Alive))
            {
                return;
            }

            this.sendUpdateTimer += gameTime.ElapsedGameTime;
            if (this.sendUpdateTimer.CheckElapsed())
            {
                MouseState mouseState = Mouse.GetState();
                if (windowBounds.Contains(mouseState.Position))
                {
                    Point diff = mouseState.Position - this.neighbourhoodRect.Center;
                    double angle = Math.Atan2(diff.Y, diff.X);
                    byte direction = AngleHelper.RadiansToDirection(angle);

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        client.Update(direction, direction);
                    }
                    else
                    {
                        client.Update(direction);
                    }
                }
            }

            this.getCurrentTimer += gameTime.ElapsedGameTime;
            if (this.getCurrentTimer.CheckElapsed())
            {
                this.current = client.GetCurrent();
            }

            this.getNeighboursTimer += gameTime.ElapsedGameTime;
            if (this.getNeighboursTimer.CheckElapsed())
            {
                this.neighbours = client.GetNeighbours();
            }

            this.getRadarTimer += gameTime.ElapsedGameTime;
            if (this.getRadarTimer.CheckElapsed())
            {
                this.radar = client.GetRadar();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this.configuration.Colors.Background);

            if (this.current == null)
            {
                return;
            }

            spriteBatch.Begin();

            if (this.current.State.HasFlag(DotState.Alive))
            {
                if (this.neighbours != null)
                {
                    foreach (var dot in this.neighbours.Neighbours)
                    {
                        if (dot.State.HasFlag(DotState.Beaming))
                        {
                            DrawBeam(spriteBatch, dot.RX, dot.RY, dot.BeamDirection);
                        }

                        DrawDot(spriteBatch, dot);
                    }
                }

                if (this.radar != null)
                {
                    foreach (var dot in this.radar.RadarDots)
                    {
                        DrawRadarPoint(spriteBatch, dot.RX, dot.RY, dot.Team);
                    }
                }
            }
            else
            {
                Window.Title = "End";
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDot(SpriteBatch spriteBatch, NeighbourDot dot)
        {
            Vector2 dotCenter = ToNeighbourhoodVector(dot.RX, dot.RY);
            Rectangle dotRect = new Rectangle((dotCenter - (dotSize / 2)).ToPoint(), dotSize.ToPoint());
            spriteBatch.Draw(dotTexture, dotRect, teamColors[dot.Team]);
        }

        private void DrawBeam(SpriteBatch spriteBatch, ushort rx, ushort ry, byte direction)
        {
            Vector2 beamStart = ToNeighbourhoodVector(rx, ry);
            spriteBatch.Draw(this.beamTexture, new Rectangle(beamStart.ToPoint(), this.beamSize.ToPoint()), null, this.beamColor, (float)AngleHelper.DirectionToRadians(direction), new Vector2(0, this.beamTexture.Height / 2), SpriteEffects.None, 0f);
        }

        private void DrawRadarPoint(SpriteBatch spriteBatch, byte rx, byte ry, int team)
        {
            Vector2 dotCenter = ToRadarVector(rx, ry);
            Rectangle dotRect = new Rectangle((dotCenter - (radarDotSize / 2)).ToPoint(), radarDotSize.ToPoint());
            spriteBatch.Draw(dotTexture, dotRect, teamColors[team]);
        }

        private Vector2 ToNeighbourhoodVector(ushort rx, ushort ry)
        {
            return neighbourhoodRect.Location.ToVector2() +
                (new Vector2(rx, ry) * Constants.UI.NeighbourhoodSize / ushort.MaxValue);
        }

        private Vector2 ToRadarVector(byte rx, byte ry)
        {
            return radarRect.Location.ToVector2() +
                (new Vector2(rx, ry) * Constants.UI.RadarSize / byte.MaxValue);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.client.Dispose();
            }
        }
    }
}