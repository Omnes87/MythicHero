namespace MythicHero
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class GameLoopContext
    {
        public GameLoopContext(GameTime gameTime)
        {
            this.GameTime = gameTime;
            this.KeyboardState = Keyboard.GetState();
            this.GamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public GameTime GameTime { get; private set; }

        public KeyboardState KeyboardState { get; private set; }

        public GamePadState GamePadState { get; private set; }
    }
}
