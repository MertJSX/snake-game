using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SnakeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Game game = new Game();

            while (!game.isGameOver)
            {
                var inputTask = Task.Run(() => GetInput(game));
                Thread.Sleep(50);
                game.GameTick();
            }
            Thread.Sleep(1000);
            Console.WriteLine("GAME OVER!");
        }

        static void GetInput(Game game)
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        game.SetMoveDirection(MoveDirection.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        game.SetMoveDirection(MoveDirection.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        game.SetMoveDirection(MoveDirection.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        game.SetMoveDirection(MoveDirection.Right);
                        break;
                }
            }
        }
    }
}
