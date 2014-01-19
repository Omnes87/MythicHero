namespace MythicHero.GameModes
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class GameModeBase
    {
        private bool isContentLoaded;

        protected SharedAssets SharedAssets { get; private set; }

        public void Initialize(SharedAssets sharedAssets)
        {
            this.SharedAssets = sharedAssets;
        }

        public void Load(ContentManager contentManager)
        {
            if (!this.isContentLoaded)
            {
                this.LoadContent(contentManager);
                this.isContentLoaded = true;
            }
        }

        public void Unload(ContentManager contentManager)
        {
            if (this.isContentLoaded)
            {
                contentManager.Unload();
                this.UnloadContent();
                this.isContentLoaded = false;
            }
        }

        public abstract GameModeBase Update(GameLoopContext context);

        public abstract void Draw(SpriteBatch spriteBatch);

        protected abstract void LoadContent(ContentManager contentManager);

        protected abstract void UnloadContent();
    }
}
