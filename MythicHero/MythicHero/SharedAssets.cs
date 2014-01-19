namespace MythicHero
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class SharedAssets
    {
        /// <summary>
        /// The font used to display UI elements
        /// </summary>
        public SpriteFont Font { get; private set; }

        public SharedAssets(ContentManager contentManager)
        {
            this.Font = contentManager.Load<SpriteFont>("gameFont");
        }
    }
}
