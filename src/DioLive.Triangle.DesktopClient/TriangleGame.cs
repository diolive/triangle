using System;
using System.IO;
using System.Threading.Tasks;

using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DesktopClient.Configuration;
using DioLive.Triangle.DesktopClient.GameObjects;

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

        private ClientWorker clientWorker;

        private bool destroyed;

        private CurrentResponse current;
        private Neighbourhood neighbourhood;
        private Radar radar;

        private ManualTimer sendUpdateTimer;

        public TriangleGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Environment.CurrentDirectory, "Content");

            this.windowWidth = Constants.UI.NeighbourhoodSize + Constants.UI.RadarSize;
            this.windowHeight = Constants.UI.NeighbourhoodSize;

            this.graphics.PreferredBackBufferWidth = this.windowWidth;
            this.graphics.PreferredBackBufferHeight = this.windowHeight;

            Window.Title = "Triangle";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
                        this.clientWorker.UpdateAsync(direction, direction).AsSync();
                    }
                    else
                    {
                        this.clientWorker.UpdateAsync(direction).AsSync();
                    }
                }
            };

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.configuration = ConfigurationLoader.Load("appconfig.json");

            Assets.Load(Content);

            this.neighbourhood = new Neighbourhood(this.configuration);
            this.radar = new Radar(this.configuration);

            this.clientWorker = new ClientWorker(this.configuration.ServerUri);

            this.clientWorker.UpdateCurrent += response => this.current = response;
            this.clientWorker.UpdateNeighbours += response => this.neighbourhood.CurrentResponse = response;
            this.clientWorker.UpdateRadar += response => this.radar.CurrentResponse = response;
            this.clientWorker.Destroyed += () =>
            {
                this.destroyed = true;
            };

            this.clientWorker.ActivateAsync().AsSync();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
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

            if (this.destroyed || (this.current != null && !this.current.State.HasFlag(DotState.Alive)))
            {
                return;
            }

            this.sendUpdateTimer += gameTime.ElapsedGameTime;

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

            if (this.destroyed || !this.current.State.HasFlag(DotState.Alive))
            {
                Window.Title = "End";
                return;
            }

            this.spriteBatch.Begin();

            this.neighbourhood.Draw(this.spriteBatch);
            this.radar.Draw(this.spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.clientWorker.Dispose();
            }
        }
    }
}