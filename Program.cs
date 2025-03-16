using System.Text;

namespace SnakeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Change the output encoding of the Console
            Game game = new Game(); // Create new Game object

            var inputTask = Task.Run(() => GetInput(game)); // Run the GetInput method asynchronously
            while (!game.isGameOver) // Repeat until the player tries to eat themselves xd
            {
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
                // Read keyboard key
                var key = Console.ReadKey(intercept: true).Key;

                // Determine which key is pressed then set the move direction of the snake.
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
