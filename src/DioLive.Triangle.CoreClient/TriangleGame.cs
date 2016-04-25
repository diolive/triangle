using System;
using System.IO;
using System.Linq;
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

        private Color background;
        private Color[] teamColors;
        private Texture2D dotTexture;
        private Texture2D beamTexture;
        private Point center;
        private Point radarCenter;
        private Rectangle windowBounds;

        private Client client;

        private StateResponse state;

        public TriangleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Environment.CurrentDirectory, "Content");

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
            this.center = new Point(viewport.Width / 2, viewport.Height / 2);
            this.radarCenter = new Point(viewport.Width - 50, 50);
            this.windowBounds = new Rectangle(Point.Zero, GraphicsDevice.Viewport.Bounds.Size);

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

            this.client = new Client(new Uri(serverUri));

            background = ParseColor(configuration.Colors.Background);
            teamColors = configuration.Colors.Teams.Select(ParseColor).ToArray();

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

            // TODO: Add your update logic here
            var position = Mouse.GetState().Position;
            if (windowBounds.Contains(position))
            {
                var diff = position - this.center;
                var angle = (float)Math.Atan2(diff.Y, diff.X);
                client.Update(angle);
            }

            this.state = client.GetState();

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

            foreach (var dot in this.state.Neighbours)
            {
                DrawDot(spriteBatch, dot.OffsetX, dot.OffsetY, dot.Team);
            }

            foreach (var dot in this.state.Radar)
            {
                DrawRadarPoint(spriteBatch, dot.OffsetX, dot.OffsetY, dot.Team);
            }
            this.Window.Title = $"{state.Current.X} : {state.Current.Y}";

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDot(SpriteBatch spriteBatch, int x, int y, int team)
        {
            spriteBatch.Draw(dotTexture, new Rectangle(center.X + x - 25, center.Y + y - 25, 50, 50), teamColors[team]);
        }

        private void DrawRadarPoint(SpriteBatch spriteBatch, int x, int y, int team)
        {
            spriteBatch.Draw(dotTexture, new Rectangle(radarCenter.X + x / 50 - 2, radarCenter.Y + y / 50 - 2, 4, 4), teamColors[team]);
        }

        private static Color ParseColor(string hexColor)
        {
            return new Color { PackedValue = Convert.ToUInt32(hexColor, 16) };
        }
    }
}