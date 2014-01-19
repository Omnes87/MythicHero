namespace MythicHero.Field
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Map
    {
        private readonly Texture2D[,] tiles;

        public Map(Texture2D[,] tiles)
        {
            this.tiles = tiles;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var numRows = this.tiles.GetLength(0);
            var numCols = this.tiles.GetLength(1);
            for (var x = 0; x < numRows; x++)
            {
                for (var y = 0; y < numCols; y++)
                {
                    var mapTile = this.tiles[x, y];
                    spriteBatch.Draw(mapTile, new Vector2(mapTile.Width * y, mapTile.Height * x), Color.White);
                }
            }
        }
    }
}
