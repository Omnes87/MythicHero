namespace MythicHero.Field
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class MapFactory
    {
        private const int TilesInRow = 32;

        private const int TilesInColumn = 24;

        public readonly Dictionary<string, Texture2D> textures; 

        private readonly Dictionary<string, Map> maps;

        private readonly ContentManager contentManager;

        public MapFactory(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            this.textures = new Dictionary<string, Texture2D>();
            this.maps = new Dictionary<string, Map>
            {
                { "Map1", CreateMap("Map1") }
            };
        }

        public Map Create(string mapName)
        {
            Map map;
            if (!this.maps.TryGetValue(mapName, out map))
            {
                throw new InvalidOperationException("Could not find map: " + mapName);
            }

            return map;
        }

        private Map CreateMap(string mapName)
        {
            var tiles = new Texture2D[TilesInColumn, TilesInRow];
            var mapDataStream = typeof(MapFactory).Assembly.GetManifestResourceStream("MythicHero.Field.Data." + mapName + ".xml");
            using (var xmlReader = XmlReader.Create(mapDataStream))
            {
                xmlReader.ReadStartElement("Map");

                for (int x = 0; x < TilesInColumn; x++)
                {
                    xmlReader.ReadStartElement("Row");
                    for (int y = 0; y < TilesInRow; y++)
                    {
                        xmlReader.ReadStartElement("Tile");

                        var textureName = xmlReader.ReadString();
                        tiles[x, y] = this.GetOrCreateTexture(textureName);
                        
                        xmlReader.ReadEndElement();
                    }

                    xmlReader.ReadEndElement();
                }
            }

            return new Map(tiles);
        }

        private Texture2D GetOrCreateTexture(string textureName)
        {
            Texture2D texture;
            if (!this.textures.TryGetValue(textureName, out texture))
            {
                texture = this.contentManager.Load<Texture2D>("Image/Field/" + textureName);
                this.textures.Add(textureName, texture);
            }

            return texture;
        }
    }
}
