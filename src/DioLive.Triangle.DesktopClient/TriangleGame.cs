using System;
using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DesktopClient.Configuration;
using DioLive.Triangle.DesktopClient.GameObjects;
using DioLive.Triangle.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DioLive.Triangle.DesktopClient
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

        private IConfiguration configuration;

        private Rectangle windowBounds;

        private ServerClientBase client;

        private CurrentResponse current;

        private ManualTimer sendUpdateTimer;
        private ManualTimer getCurrentTimer;

        private Neighbourhood neighbourhood;
        private Radar radar;

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
            this.windowBounds = new Rectangle(0, 0, this.windowWidth, this.windowHeight);

            this.sendUpdateTimer = new ManualTimer(Constants.Timers.SendUpdateInterval);
            this.sendUpdateTimer.Tick += (s, e) =>
            {
                MouseState mouseState = Mouse.GetState();
                if (this.windowBounds.Contains(mouseState.Position))
                {
                    Point diff = mouseState.Position - this.neighbourhood.Bounds.Center;
                    double angle = Math.Atan2(diff.Y, diff.X);
                    byte direction = AngleHelper.RadiansToDirection(angle);

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        this.client.Update(direction, direction);
                    }
                    else
                    {
                        this.client.Update(direction);
                    }
                }
            };

            this.getCurrentTimer = new ManualTimer(Constants.Timers.GetCurrentInterval);
            this.getCurrentTimer.Tick += (s, e) =>
            {
                this.current = client.GetCurrent();
            };

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

            Assets.Load(this.Content);

            this.neighbourhood = new Neighbourhood(this.configuration, this.client);
            this.radar = new Radar(this.configuration, this.client);
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
            this.getCurrentTimer += gameTime.ElapsedGameTime;

            this.neighbourhood.Update(gameTime);
            this.radar.Update(gameTime);

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
                this.neighbourhood.Draw(spriteBatch);
            }
            else
            {
                Window.Title = "End";
            }

            this.radar.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
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