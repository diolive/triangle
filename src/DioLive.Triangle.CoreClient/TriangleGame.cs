using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
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
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private readonly int windowWidth;
        private readonly int windowHeight;

        private Color background;
        private Color[] teamColors;
        private Texture2D dotTexture;
        private Texture2D beamTexture;
        private Point center;
        private Point radarCenter;
        private Rectangle windowBounds;
        private Point beamSize;
        private Color beamColor;

        private ClientBase client;

        private StateResponse state;

        private GameTimer sendUpdateTimer;
        private GameTimer getStateTimer;

        public TriangleGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Environment.CurrentDirectory, "Content");

            this.windowWidth = Constants.UI.NeighbourhoodSize + 2 * Constants.UI.DotRadius + Constants.UI.RadarSize;
            this.windowHeight = Constants.UI.NeighbourhoodSize + 2 * Constants.UI.DotRadius;

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
            this.center = new Point(Constants.UI.NeighbourhoodSize / 2, Constants.UI.NeighbourhoodSize / 2);
            this.radarCenter = new Point(this.windowWidth - Constants.UI.RadarSize / 2, Constants.UI.RadarSize / 2);
            this.windowBounds = new Rectangle(0, 0, this.windowWidth, this.windowHeight);
            this.beamSize = new Point(Constants.UI.BeamLength, Constants.UI.BeamWidth);

            this.sendUpdateTimer = new GameTimer(Constants.Timers.SendUpdateInterval);
            this.getStateTimer = new GameTimer(Constants.Timers.GetStateInterval);

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
            Configuration configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("appconfig.json"));

            string serverUri = configuration.ServerUri;
            if (!serverUri.EndsWith("/"))
            {
                serverUri += "/";
            }
            if (!serverUri.StartsWith("http://") && !serverUri.StartsWith("https://"))
            {
                serverUri = "http://" + serverUri;
            }

            this.client = new JsonClient(new Uri(serverUri));

            background = ParseColor(configuration.Colors.Background);
            teamColors = configuration.Colors.Teams.Select(ParseColor).ToArray();
            beamColor = ParseColor(configuration.Colors.Beam);

            dotTexture = Content.Load<Texture2D>("dot");
            beamTexture = Content.Load<Texture2D>("rounded");
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
                Exit();

            if (this.state != null && this.state.Current.State == DotState.Destroyed)
            {
                return;
            }

            this.sendUpdateTimer += gameTime.ElapsedGameTime;
            if (this.sendUpdateTimer.CheckElapsed())
            {
                MouseState mouseState = Mouse.GetState();
                if (windowBounds.Contains(mouseState.Position))
                {
                    var diff = mouseState.Position - this.center;
                    var angle = (float)Math.Atan2(diff.Y, diff.X);

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        client.UpdateAsync(angle, angle).Forget();
                        //client.Update(angle, angle);
                    }
                    else
                    {
                        client.UpdateAsync(angle).Forget();
                        //client.Update(angle);
                    }
                }
            }

            this.getStateTimer += gameTime.ElapsedGameTime;
            if (this.getStateTimer.CheckElapsed())
            {
                client.GetStateAsync(state => { this.state = state; }).Forget();
                //this.state = client.GetState();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (this.state != null)
            {
                if (this.state.Current.State != DotState.Destroyed)
                {
                    foreach (var dot in this.state.Neighbours)
                    {
                        if (dot.Beaming.HasValue)
                        {
                            DrawBeam(spriteBatch, dot.OffsetX, dot.OffsetY, dot.Beaming.Value);
                        }
                        DrawDot(spriteBatch, dot.OffsetX, dot.OffsetY, dot.Team);
                    }

                    foreach (var dot in this.state.Radar)
                    {
                        DrawRadarPoint(spriteBatch, dot.OffsetX, dot.OffsetY, dot.Team);
                    }

#if DEBUG
                    this.Window.Title = $"{this.state.Current.X} : {this.state.Current.Y}";
#endif
                }
                else
                {
                    Window.Title = "End";
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDot(SpriteBatch spriteBatch, int x, int y, int team)
        {
            spriteBatch.Draw(dotTexture, new Rectangle(center.X + x - Constants.UI.DotRadius, center.Y + y - Constants.UI.DotRadius, 2 * Constants.UI.DotRadius, 2 * Constants.UI.DotRadius), teamColors[team]);
        }

        private void DrawBeam(SpriteBatch spriteBatch, int x, int y, float direction)
        {
            spriteBatch.Draw(beamTexture, new Rectangle(center.X + x, center.Y + y, beamSize.X, beamSize.Y), null, this.beamColor, direction, new Vector2(0, beamTexture.Height / 2), SpriteEffects.None, 0f);
        }

        private void DrawRadarPoint(SpriteBatch spriteBatch, int x, int y, int team)
        {
            spriteBatch.Draw(dotTexture, new Rectangle(radarCenter.X + x * Constants.UI.RadarSize / Constants.Space.RadarScope - Constants.UI.RadarDotRadius, radarCenter.Y + y * Constants.UI.RadarSize / Constants.Space.RadarScope - Constants.UI.RadarDotRadius, 2 * Constants.UI.RadarDotRadius, 2 * Constants.UI.RadarDotRadius), teamColors[team]);
        }

        private static Color ParseColor(string hexColor)
        {
            byte r;
            byte g;
            byte b;
            byte a;

            switch (hexColor.Length)
            {
                case 8:
                    a = Convert.ToByte(hexColor.Substring(0, 2), 16);
                    r = Convert.ToByte(hexColor.Substring(2, 2), 16);
                    g = Convert.ToByte(hexColor.Substring(4, 2), 16);
                    b = Convert.ToByte(hexColor.Substring(6, 2), 16);
                    break;

                case 6:
                    a = 255;
                    r = Convert.ToByte(hexColor.Substring(0, 2), 16);
                    g = Convert.ToByte(hexColor.Substring(2, 2), 16);
                    b = Convert.ToByte(hexColor.Substring(4, 2), 16);
                    break;

                case 3:
                    a = 255;
                    r = Convert.ToByte(new string(hexColor[0], 2), 16);
                    g = Convert.ToByte(new string(hexColor[1], 2), 16);
                    b = Convert.ToByte(new string(hexColor[2], 2), 16);
                    break;

                default:
                    throw new ArgumentException($"Unknown color definition: {hexColor}", nameof(hexColor));
            }

            return new Color(r, g, b, a);
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