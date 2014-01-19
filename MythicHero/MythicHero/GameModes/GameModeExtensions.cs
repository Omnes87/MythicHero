namespace MythicHero.GameModes
{
    using System;
    using System.Collections.Generic;

    public static class GameModeExtensions
    {
        private static readonly Dictionary<GameMode, GameModeBase> gameModes = new Dictionary<GameMode, GameModeBase>
        {
            { GameMode.StartScreen, new StartScreen() },
            { GameMode.Field, new Field() }
        };

        public static GameModeBase GetInstance(this GameMode gameMode)
        {
            GameModeBase gameModeInstance;
            if (!gameModes.TryGetValue(gameMode, out gameModeInstance))
            {
                throw new InvalidOperationException("Invalid GameMode: " + gameMode.ToString());
            }

            return gameModeInstance;
        }
    }
}
