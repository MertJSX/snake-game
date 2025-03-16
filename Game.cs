using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    enum GameElement // There are 4 possible game elements
    {
        Empty, Food, SnakePart, SnakeHead
    }

    enum MoveDirection // enum for Move directions
    {
        Right, Left, Up, Down
    }

    class MapPosition // This class is used to store map positions like foodPosition
    {
        public int row;
        public int col;

        public MapPosition(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    class Game
    {
        public GameElement[,] gameMap = new GameElement[24, 32]; // This is game map, simply 24 is height and 32 is width of the game map
        public List<MapPosition> snake; // Positions of Snake parts
        public MapPosition foodPosition; // Current position of the food (Q)
        public MapPosition lastPositionOfTail; // This is the previous position of last tail, this is used for extending the snake
        public MoveDirection snakeMoveDirection = MoveDirection.Right; // Current move Direction of the snake.
        public bool isGameOver = false; // If isGameOver is true, game will stop.
        public int points = 0; // Usually for each food you eat you will get 10 points


        public void SetMoveDirection(MoveDirection moveDirection)
        {
            //These are validations for correct positioning. For example:
            //If our move direction is right, we can't directly turn left,
            //first we must go up or down, then left.

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
            snakeMoveDirection = moveDirection; // If moveDirection is appropriate, set the snakeMoveDirection to moveDirection
        } 


        public Game() {
            snake = new List<MapPosition>(); // Initialize the snake list.
            // set the start position of the snake
            snake.Add(new MapPosition(2, 6)); // head
            snake.Add(new MapPosition(2, 5)); // other snake parts...
            snake.Add(new MapPosition(2, 4));

            foodPosition = new MapPosition(5, 5); // set the first position of the food

            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    gameMap[i, j] = GameElement.Empty; // Make all game positions Empty
                }
            }
        }

        public MapPosition DetermineFoodPosition()
        {
            // This method will find a appropriate position for the food.
            Random random = new Random();
            MapPosition newFoodPosition = new MapPosition(random.Next(0, 23), random.Next(0, 31)); // Get random position
            if (gameMap[newFoodPosition.row, newFoodPosition.col] == GameElement.SnakePart)
            {
                return DetermineFoodPosition(); // If the random position is taken, try to get another position. Also the method is recursive.
            }
            return newFoodPosition; // All the requirements are met, then return the appropriate food position
        }

        public void SetMapElements()
        {
            List<MapPosition> earlyChangedPosses = new List<MapPosition>(); // (Hard to explain... :D)
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    bool isItChangedToSnakePart = false; // I used this varriable for a better performance
                    for (int snakeIndex = 0; snakeIndex < snake.Count; snakeIndex++)
                    {
                        if (isItChangedToSnakePart) // If we already changed the game element of this position to SnakePart or SnakeHead, the loop will break;
                        {
                            break; // Stop the loop for better performance
                        }
                        if (snake[snakeIndex].row == i && snake[snakeIndex].col == j)
                        {
                            if (snakeIndex == 0)
                            {
                                gameMap[i, j] = GameElement.SnakeHead;
                            } 
                            else
                            {
                                gameMap[i, j] = GameElement.SnakePart;
                            }
                            isItChangedToSnakePart = true; // Mark the position that it is changed to SnakePart
                            earlyChangedPosses.Add(snake[snakeIndex]);
                        }
                        else if (!earlyChangedPosses.Contains(snake[snakeIndex])) 
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

            gameMap[foodPosition.row, foodPosition.col] = GameElement.Food; // Place the food.
        }

        public void MoveSnake()
        {
            MapPosition firstMemory = snake[0];
            MapPosition secondMemory = snake[0];
            bool isOutOfRange = false;

            // Move the snake by considering snakeMoveDirection
            switch (snakeMoveDirection)
            {
                case MoveDirection.Right:
                    snake[0] = new MapPosition(snake[0].row, snake[0].col + 1);
                    break;
                case MoveDirection.Left:
                    snake[0] = new MapPosition(snake[0].row, snake[0].col - 1);
                    break;
                case MoveDirection.Up:
                    snake[0] = new MapPosition(snake[0].row - 1, snake[0].col);
                    break;
                default:
                    snake[0] = new MapPosition(snake[0].row + 1, snake[0].col);
                    break;
            }

            // Validation: If snake crosses the wall it will teleport them to the other side.
            if (snake[0].row == gameMap.GetLength(0))
            {
                snake[0].row = 0;
            }
            if (snake[0].row == -1)
            {
                snake[0].row = gameMap.GetLength(0);
            }
            if (snake[0].col == gameMap.GetLength(1))
            {
                snake[0].col = 0;
            }
            if (snake[0].col == -1)
            {
                snake[0].col = gameMap.GetLength(1);
            }

            // Control the positions to avoid possible out of range errors.
            if (
                snake[0].row < 0 || snake[0].row >= gameMap.GetLength(0) ||
                snake[0].col < 0 || snake[0].col >= gameMap.GetLength(1)
               )
            {
                isOutOfRange = true;
            }

            if (!isOutOfRange) // Avoid possible Out of range exceptions
            {
                if (gameMap[snake[0].row, snake[0].col] == GameElement.SnakePart) // This event is fired when the snake tries to eat itself
                {
                    isGameOver = true; // Game ends
                    return;
                }

                if (gameMap[snake[0].row, snake[0].col] == GameElement.Food) // This event is fired when the snake eats the food.
                {
                    points += 10; // Add 10 points
                    snake.Add(lastPositionOfTail); // Extend the snake.
                    foodPosition = DetermineFoodPosition(); // Determine a new position for the food.
                    gameMap[snake[0].row, snake[0].col] = GameElement.Empty; // Make the position empty because the existing food was eaten by the snake.
                }
            }

            // Move the snake parts.
            for (int i = 1; i < snake.Count; i++) 
            {
                // These were the most confusing three lines of code. xD
                secondMemory = snake[i];
                snake[i] = firstMemory;
                firstMemory = secondMemory;

                // Set the lastPositionOfTail
                if (i + 1 == snake.Count)
                {
                    lastPositionOfTail = firstMemory;
                }
            }
        }

        // This is the method that runs every frame.
        public void GameTick() // This method will repeat until the game ends.
        {
            /* I don't know how to order the calling of these methods. This method of
             ordering seemed correct to me. If it works don't touch it... :D  */
            RenderMap(); // Call RenderMap first
            MoveSnake(); // Move the snake
            SetMapElements(); // Then SetMapElements
        }

        public void RenderMap()
        {
            StringBuilder sb = new StringBuilder(); // We are using string builder for better performance.
            // ANSI escape codes (For more information: https://gist.github.com/fnky/458719343aabd01cfb17a3a4f7296797#8-16-colors)
            string red = "\u001b[31m";
            string green = "\u001b[32m";
            string reset = "\u001b[0m";

            SetMapElements();
            sb.AppendLine(" --------------------------------"); // Manually created top wall
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
                        case GameElement.SnakeHead:
                            switch (snakeMoveDirection)
                            {
                                case MoveDirection.Right:
                                    sb.Append(green + ">" + reset);
                                    break;
                                case MoveDirection.Left:
                                    sb.Append(green + "<" + reset);
                                    break;
                                case MoveDirection.Up:
                                    sb.Append(green + "^" + reset);
                                    break;
                                case MoveDirection.Down:
                                    sb.Append(green + "V" + reset);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            sb.Append(" ");
                            break;
                    }
                }
                sb.Append("|\n");
            }
            sb.AppendLine(" --------------------------------"); // Manually created bottom wall

            Console.SetCursorPosition(0, 0); // Set Cursor position to change existing text
            Console.WriteLine($"\tPoints: {points}"); // Print current points
            Console.SetCursorPosition(0, 2);
            Console.Write(sb); // Paste/Write created string(sb)
        }
    }
}
