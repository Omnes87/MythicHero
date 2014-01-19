namespace MythicHero.GameModes
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MythicHero.Field;

    public class Field : GameModeBase
    {
        private Map currentMap;

        public override GameModeBase Update(GameLoopContext context)
        {
            // Allows the game to exit
            if (context.KeyboardState.IsKeyDown(Keys.Escape)
                || context.GamePadState.Buttons.Back == ButtonState.Pressed)
            {
                return null;
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.currentMap.Draw(spriteBatch);
        }

        protected override void LoadContent(ContentManager contentManager)
        {
            this.currentMap = new MapFactory(contentManager).Create("Map1");
        }

        protected override void UnloadContent()
        {
        }
    }
}
