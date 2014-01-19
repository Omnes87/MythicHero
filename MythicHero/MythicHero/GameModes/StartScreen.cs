namespace MythicHero.GameModes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MythicHero.Resources;

    public class StartScreen : GameModeBase
    {
        private Texture2D opening;

        public override GameModeBase Update(GameLoopContext context)
        {
            // Allows the game to exit
            if (context.KeyboardState.IsKeyDown(Keys.Escape)
                || context.GamePadState.Buttons.Back == ButtonState.Pressed)
            {
                return null;
            }

            if (context.KeyboardState.IsKeyDown(Keys.Enter)
                || context.GamePadState.Buttons.Start == ButtonState.Pressed)
            {
                return GameMode.Field.GetInstance();
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.opening, Vector2.Zero, Color.White);

            var startString = LocStrings.StartScreen_PressEnter;
            var titleSafeArea = spriteBatch.GraphicsDevice.Viewport.TitleSafeArea;
            var startStringDimensions = this.SharedAssets.Font.MeasureString(startString);
            var startStringPosition = new Vector2((titleSafeArea.Width - startStringDimensions.X) / 2, 320);
            spriteBatch.DrawString(this.SharedAssets.Font, startString, startStringPosition, Color.White);
        }

        protected override void LoadContent(ContentManager contentManager)
        {
            this.opening = contentManager.Load<Texture2D>("Image/Opening");
        }

        protected override void UnloadContent()
        {
            this.opening = null;
        }
    }
}
