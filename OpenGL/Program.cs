namespace OpenGL
{
    class Program
    {
        static void Main(string[] args)
        {
            Fonts.GenerateFontImage();
            using (var game = new Game())
            {
                game.Run(30);
            }
        }

    }

}
