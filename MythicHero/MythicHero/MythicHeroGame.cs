namespace MythicHero
{
    using MythicHero.GameModes;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MythicHeroGame : Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;

        private GameModeBase currentGameMode;

        public MythicHeroGame()
        {
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 640,
                PreferredBackBufferHeight = 480
            };
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.currentGameMode = GameMode.StartScreen.GetInstance();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load shared assets
            var sharedAssets = new SharedAssets(this.Content);

            // Initialize initial game modes for the first time.
            // TODO: Find a better way to initialize games modes for the first time. A factory?
            GameMode.StartScreen.GetInstance().Initialize(sharedAssets);
            GameMode.Field.GetInstance().Initialize(sharedAssets);

            // Load up the initial game mode's assets.
            // TODO: Find a better way to transitions and Load/Unload content
            GameMode.StartScreen.GetInstance().Load(this.Content);
            GameMode.Field.GetInstance().Load(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            var nextGameMode = this.currentGameMode.Update(new GameLoopContext(gameTime));
            if (nextGameMode == null)
            {
                this.Exit();
            }

            this.currentGameMode = nextGameMode;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Start drawing
            this.spriteBatch.Begin();

            this.currentGameMode.Draw(this.spriteBatch);

            // Stop drawing
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
