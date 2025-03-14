using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    enum GameElement
    {
        Empty, Food, SnakePart
    }

    enum MoveDirection
    {
        Right, Left, Up, Down
    }

    class MapPosition
    {
        public int dimension;
        public int position;

        public MapPosition(int dimension, int position)
        {
            this.dimension = dimension;
            this.position = position;
        }
    }
    class Game
    {
        public GameElement[,] gameMap = new GameElement[24, 32];

        public List<MapPosition> snake;
        public MapPosition foodPosition;
        public MapPosition lastPositionOfTail;
        public MoveDirection snakeMoveDirection = MoveDirection.Right;
        public bool isGameOver = false;
        public int points = 0;


        public void SetMoveDirection(MoveDirection moveDirection)
        {
            if (snakeMoveDirection == MoveDirection.Right && moveDirection == MoveDirection.Left)
            {
                return;
            }
            if (snakeMoveDirection == MoveDirection.Left && moveDirection == MoveDirection.Right)
            {
                return;
            }
            if (snakeMoveDirection == MoveDirection.Up && moveDirection == MoveDirection.Down)
            {
                return;
            }
            if (snakeMoveDirection == MoveDirection.Down && moveDirection == MoveDirection.Up)
            {
                return;
            }
            snakeMoveDirection = moveDirection;
        } 


        public Game() {
            snake = new List<MapPosition>();
            snake.Add(new MapPosition(2, 6)); // head
            snake.Add(new MapPosition(2, 5));
            snake.Add(new MapPosition(2, 4));
            snake.Add(new MapPosition(2, 3));
            snake.Add(new MapPosition(2, 2));
            snake.Add(new MapPosition(2, 1));
            snake.Add(new MapPosition(2, 0));

            foodPosition = new MapPosition(5, 5);

            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    gameMap[i, j] = GameElement.Empty;
                }
            }
        }

        public MapPosition DetermineFoodPosition()
        {
            Random random = new Random();
            MapPosition newFoodPosition = new MapPosition(random.Next(0, 23), random.Next(0, 31));
            if (gameMap[newFoodPosition.dimension, newFoodPosition.position] == GameElement.SnakePart)
            {
                return DetermineFoodPosition();
            }
            return newFoodPosition;
        }

        public void SetMapElements()
        {
            List<MapPosition> earlyChangedPosses = new List<MapPosition>();
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    bool isItChangedToSnakePart = false;
                    foreach (MapPosition pos in snake)
                    {
                        if (isItChangedToSnakePart)
                        {
                            break;
                        }
                        if (pos.dimension == i && pos.position == j)
                        {
                            gameMap[i, j] = GameElement.SnakePart;
                            isItChangedToSnakePart = true;
                            earlyChangedPosses.Add(pos);
                        } 
                        else if (!earlyChangedPosses.Contains(pos))
                        {
                            gameMap[i, j] = GameElement.Empty;
                        }
                    }
                    if (!isItChangedToSnakePart)
                    {
                        gameMap[i, j] = GameElement.Empty;
                    }
                }
            }

            gameMap[foodPosition.dimension, foodPosition.position] = GameElement.Food;
        }

        public void MoveSnake()
        {
            MapPosition firstMemory = snake[0];
            MapPosition secondMemory = snake[0];
            bool isOutOfRange = false;

            switch (snakeMoveDirection)
            {
                case MoveDirection.Right:
                    snake[0] = new MapPosition(snake[0].dimension, snake[0].position + 1);
                    break;
                case MoveDirection.Left:
                    snake[0] = new MapPosition(snake[0].dimension, snake[0].position - 1);
                    break;
                case MoveDirection.Up:
                    snake[0] = new MapPosition(snake[0].dimension - 1, snake[0].position);
                    break;
                default:
                    snake[0] = new MapPosition(snake[0].dimension + 1, snake[0].position);
                    break;
            }

            if (snake[0].dimension == gameMap.GetLength(0))
            {
                snake[0].dimension = 0;
            }
            if (snake[0].dimension == -1)
            {
                snake[0].dimension = gameMap.GetLength(0);
            }
            if (snake[0].position == gameMap.GetLength(1))
            {
                snake[0].position = 0;
            }
            if (snake[0].position == -1)
            {
                snake[0].position = gameMap.GetLength(1);
            }

            if (
                snake[0].dimension < 0 || snake[0].dimension >= gameMap.GetLength(0) ||
                snake[0].position < 0 || snake[0].position >= gameMap.GetLength(1)
               )
            {
                isOutOfRange = true;
            }

            if (!isOutOfRange)
            {
                if (gameMap[snake[0].dimension, snake[0].position] == GameElement.SnakePart)
                {
                    isGameOver = true;
                    return;
                }

                if (gameMap[snake[0].dimension, snake[0].position] == GameElement.Food)
                {
                    points += 10;
                    snake.Add(lastPositionOfTail);
                    foodPosition = DetermineFoodPosition();
                    gameMap[snake[0].dimension, snake[0].position] = GameElement.Empty;
                }
            }


            for (int i = 1; i < snake.Count; i++)
            {
                secondMemory = snake[i];
                snake[i] = firstMemory;
                firstMemory = secondMemory;

                if (i + 1 == snake.Count)
                {
                    lastPositionOfTail = firstMemory;
                }
            }
        }

        public void GameTick()
        {
            MoveSnake();
            SetMapElements();
            RenderMap();
        }


        public void RenderMap()
        {
            StringBuilder sb = new StringBuilder();
            string red = "\u001b[31m";
            string green = "\u001b[32m";
            string reset = "\u001b[0m";

            SetMapElements();
            sb.AppendLine(" --------------------------------");
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                sb.Append("|");
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    switch (gameMap[i, j])
                    {
                        case GameElement.Food:
                            sb.Append(red + "Q" + reset);
                            break;
                        case GameElement.SnakePart:
                            sb.Append(green + "O" + reset);
                            break;
                        default:
                            sb.Append(" ");
                            break;
                    }
                }
                sb.Append("|\n");
            }
            sb.AppendLine(" --------------------------------");

            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"\tPoints: {points}");
            Console.SetCursorPosition(0, 2);
            Console.Write(sb);
        }
    }
}
