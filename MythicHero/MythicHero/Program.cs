namespace MythicHero
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MythicHeroGame game = new MythicHeroGame())
            {
                game.Run();
            }
        }
    }
#endif
}

