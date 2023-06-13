using BattleShip123Core.Interfaces;
using BattleShip123Core.Model;
using BattleShip123View;
using static BattleShip123Core.Interfaces.IEngine;

namespace BattleShip123Engine.Models
{
    public class Engine : IEngine
    {
        public Engine(IView view, Player playerOne, Player playerTwo)
        {
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;
            this.view = view;
            NewGame();
        }

        readonly Player playerOne;
        readonly Player playerTwo;
        readonly IView view;
        readonly IComputeConstraints constraint = new ComputeConstraints(Grid.Size);
        bool move;

        /// <summary>
        /// Reset players boards and ships
        /// </summary>
        public void NewGame()
        {
            PlaceShips(playerOne);
            PlaceShips(playerTwo);
        }

        /// <summary>
        /// Start a new game
        /// </summary>
        public void StartGame()
        {
            try
            {
                Player current = playerOne;
                view.DrawShips(playerOne);
                move = true;
                const int noShipsLeft = 0;
                do
                {
                    try
                    {
                        view.DrawShips(current);
                        var inputText = Console.ReadLine();
                        (int x, int y) = ParseInput(inputText);
                        if (move)
                        {
                            move = playerOne.ShotShip(x, y);
                        }
                        else
                        {
                            current = playerTwo;
                            move = !playerTwo.ShotShip(x, y);
                        }
                        current = move ? playerOne : playerTwo;
                    }
                    catch (ArgumentException arg)
                    {
                        Console.WriteLine(arg.Message);
                        Console.ReadLine();
                    }
                    catch
                    {
                        throw;
                    }
                }
                while (playerOne.sumOfShipCells != noShipsLeft && playerTwo.sumOfShipCells != noShipsLeft);
                var winner = playerOne.sumOfShipCells == noShipsLeft ? playerTwo.Name : playerOne.Name;
                Console.WriteLine($"Congratulations {winner} is a winner!");
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        /// <summary>
        /// Place ships on the board for a given player
        /// </summary>
        /// <param name="player">Player on which board ships will be placed</param>
        public void PlaceShips(Player player)
        {
            player.Reset();
            var random = new Random();
            int randomX;
            int randomY;
            ShipOrientation orientation;
            foreach (var shipSize in player.ships)
            {
                do
                {
                    randomX = random.Next(0, Grid.Size);
                    randomY = random.Next(0, Grid.Size);
                    orientation = (ShipOrientation)(random.Next(0, 100) % 2);
                }
                while (!PlaceShip(player, new Ship() { X = randomX, Y = randomY, Size = shipSize, Orientation = orientation }));
            }
        }

        /// <summary>
        /// Place ship on a players board
        /// </summary>
        /// <param name="player">Player on which board ship will be placed</param>
        /// <param name="ship">Ship to place</param>
        /// <returns>True if placing was successfull</returns>
        public bool PlaceShip(Player player, Ship ship)
        {
            if (!IsPlacingShipPossible(player, ship))
            {
                return false;
            }

            var range = new
            {
                x = ship.Orientation == ShipOrientation.Horizontal ? ship.Size + ship.X : ship.X + 1,
                y = ship.Orientation == ShipOrientation.Vertical ? ship.Size + ship.Y : ship.Y + 1,
            };

            for (var xPosition = ship.X; xPosition < range.x; ++xPosition)
            {
                for (var yPosition = ship.Y; yPosition < range.y; ++yPosition)
                {
                    var coordinate = yPosition * Grid.Size + xPosition;
                    player.Grid[coordinate] = Grid.ShipCell;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if we can put ship in this location
        /// </summary>
        /// <param name="player">Player on which board ship will be placed</param>
        /// <param name="ship">Ship to place</param>
        /// <returns>true if placing is posible</returns>
        public bool IsPlacingShipPossible(Player player, Ship ship)
        {
            if (!IsInRange(ship.X, ship.Y))
            {
                return false;
            }
            if (!IsShipFittingToGrid(ship))
            {
                return false;
            }
            if (IsColision(player, ship))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse user input 
        /// </summary>
        /// <param name="inputText">Input string to parse</param>
        /// <returns>Returns X, Y coordinates</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception if argument was out of specified range</exception>
        /// <exception cref="ArgumentException">Throws exception if argument was wrong</exception>
        public (int, int) ParseInput(string inputText)
        {
            const int inputLenght = 2;
            if (inputText?.Length >= inputLenght)
            {
                var x = char.ToLower(inputText[0]) - 'a';
                var y = inputText[1] - '1';
                if (inputText.Length > inputLenght)
                {
                    var rowString = inputText.Substring(1, inputLenght);
                    int.TryParse(rowString, out y);
                    --y;
                }

                var yRange = 'a' + Grid.Size;

                if (y < 0 || y >= Grid.Size)
                {
                    throw new ArgumentOutOfRangeException($"X coordinate should be in range from 1 to {Grid.Size}");
                }
                if (x < 'a' && x >= Grid.Size)
                {
                    throw new ArgumentOutOfRangeException($"Y coordinate should be in range from A to {yRange}");
                }
                return (x, y);
            }
            throw new ArgumentException("Coordinate was to short");
        }

        private static bool IsInRange(int x, int y)
        {
            if (x < 0 || x >= Grid.Size)
            {
                return false;
            }
            if (y < 0 || y >= Grid.Size)
            {
                return false;
            }
            return true;
        }

        private static bool IsShipFittingToGrid(Ship ship)
        {
            // If we put ship with size 4 in the x or y equal 0, last cell will be in 3
            // so we need to substract 1
            const int normalizePosition = 1;
            if (ship.Orientation == ShipOrientation.Horizontal)
            {
                var lastShipCell = ship.X + ship.Size - normalizePosition;
                return IsInRange(lastShipCell, ship.Y);
            }
            else
            {
                var lastShipCell = ship.Y + ship.Size - normalizePosition;
                return IsInRange(ship.X, lastShipCell);
            }
        }

        private bool IsColision(Player player, Ship ship)
        {
            var constraints = constraint.GetLoopConstraints(ship);

            for (var xPosition = constraints.xLoopStartPos; xPosition < constraints.xStopCondition; xPosition++)
            {
                for (var yPosition = constraints.yLoopStartPos; yPosition < constraints.yStopCondition; yPosition++)
                {
                    var coordinate = yPosition * Grid.Size + xPosition;
                    if (player.Grid[coordinate] != Grid.EmptyCell)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
