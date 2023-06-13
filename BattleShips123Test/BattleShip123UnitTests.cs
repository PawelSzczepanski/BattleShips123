using BattleShip123Core.Model;
using BattleShip123Engine.Models;
using BattleShip123View;
using static BattleShip123Core.Interfaces.IEngine;

namespace BattleShips123Test
{
    public class BattleShip123UnitTests
    {
        Player playerOne;
        Player playerTwo;
        Engine engine;
        IView view;

        public BattleShip123UnitTests()
        {
            playerOne = new Player("Test player");
            playerTwo = new Player("Test player");
            view = new View();
            engine = new Engine(view, playerOne, playerTwo);
            playerOne.Reset();
            playerTwo.Reset();
            var allCellsEmpty = playerOne.Grid.GameBoard.All(x => x == Grid.EmptyCell);
            Assert.True(allCellsEmpty);
        }

        [Fact]
        public void PutShipInRange_Success()
        {
            int rangeX = 10;
            int rangeY = 10;
            int[] shipSizes = { 3, 4 };
            ShipOrientation[] orientations = { ShipOrientation.Vertical, ShipOrientation.Horizontal };

            foreach (var shipSize in shipSizes)
            {
                foreach (var orientation in orientations)
                {
                    if (orientation == ShipOrientation.Vertical)
                    {
                        rangeY -= shipSize;
                    }
                    else
                    {
                        rangeX -= shipSize;
                    }
                    for (int x = 0; x < rangeX; x++)
                    {
                        for (int y = 0; y < rangeY; y++)
                        {
                            var result = engine.IsPlacingShipPossible(playerOne, new Ship() { X = x, Y = y, Size = shipSize, Orientation = orientation });
                            Assert.True(result);
                        }
                    }
                }
            }
        }

        [Fact]
        public void CheckRanges_Success()
        {
            const int shipSize = 4;
            var constraint = new ComputeConstraints(Grid.Size);
            var ranges = new[]
            {
                new {
                        xStartPos = 0,
                        yStartPost = 0,
                        xExpectedStartPost = 0,
                        yExpectedStartPost = 0,
                        xRange = 5,
                        yRange = 2,
                    },
                new {
                        xStartPos = 1,
                        yStartPost = 1,
                        xExpectedStartPost = 0,
                        yExpectedStartPost = 0,
                        xRange = 6,
                        yRange = 3,
                    },
                new {
                        xStartPos = 0,
                        yStartPost = 9,
                        xExpectedStartPost = 0,
                        yExpectedStartPost = 8,
                        xRange = 5,
                        yRange = Grid.Size,
                    },
                new {
                        xStartPos = 6,
                        yStartPost = 0,
                        xExpectedStartPost = 5,
                        yExpectedStartPost = 0,
                        xRange = Grid.Size,
                        yRange = 2,
                    },
                new {
                        xStartPos = 6,
                        yStartPost = 9,
                        xExpectedStartPost = 5,
                        yExpectedStartPost = 8,
                        xRange = Grid.Size,
                        yRange = Grid.Size,
                    },
            };

            foreach (var range in ranges)
            {
                var ship = new Ship() { X = range.xStartPos, Y = range.yStartPost, Size = shipSize, Orientation = ShipOrientation.Horizontal };
                var constraints = constraint.GetLoopConstraints(ship);
                Assert.True(range.xRange == constraints.xStopCondition);
                Assert.True(range.yRange == constraints.yStopCondition);
                Assert.True(range.xExpectedStartPost == constraints.xLoopStartPos);
                Assert.True(range.yExpectedStartPost == constraints.yLoopStartPos);
            }
        }

        [Fact]
        public void ShotShip_Success()
        {
            engine.PlaceShip(playerOne, new Ship() { X = 5, Y = 5, Size = 4, Orientation = ShipOrientation.Horizontal });
            Assert.True(playerOne.ShotShip(5, 5));
            Assert.True(playerOne.ShotShip(6, 5));
            Assert.True(playerOne.ShotShip(7, 5));
            Assert.True(playerOne.ShotShip(8, 5));
            Assert.False(playerOne.ShotShip(3, 3));
        }

        [Fact]
        public void ShipCellProperlyPlacedHorizontal_Success()
        {
            var firstShip = new Ship
            {
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };

            int[] shipCells = { 55, 56, 57, 58 };
            Assert.Contains(Grid.EmptyCell, playerOne.Grid.GameBoard);
            var result = engine.PlaceShip(playerOne, firstShip);
            Assert.True(result);
            foreach (var cell in shipCells)
            {
                Assert.True(playerOne.Grid[cell] == Grid.ShipCell);
            }
        }

        [Fact]
        public void ShipCellProperlyPlacedVertical_Success()
        {
            var firstShip = new Ship
            {
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Vertical,
            };

            int[] shipCells = { 55, 65, 75, 85 };
            var result = engine.PlaceShip(playerOne, firstShip);
            Assert.True(result);
            foreach (var cell in shipCells)
            {
                Assert.True(playerOne.Grid[cell] == Grid.ShipCell);
            }
        }

        [Fact]
        public void NewGame_Success()
        {
            engine.PlaceShip(playerOne, new Ship() { X = 5, Y = 5, Size = 4, Orientation = ShipOrientation.Vertical });
            engine.NewGame();
            var placedShips = playerOne.Grid.GameBoard.Count(x => x != Grid.EmptyCell);
            Assert.True(placedShips == playerOne.sumOfShipCells);
        }

        [Fact]
        public void ParseInput_Success()
        {
            char[] columns = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
            int[] rows = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            for (int column = 0; column < columns.Length; column++) 
            {
                for (int row = 0; row < rows.Length; row++)
                {
                    string inputString = columns[column] + rows[row].ToString();
                    (int resultX, int resultY) = engine.ParseInput(inputString);

                    int currentColumn = column % Grid.Size;
                    Assert.Equal(row, resultY);
                    Assert.Equal(currentColumn, resultX);
                }
            }
        }

        [Fact]
        public void ParseInput_ThrowArgumentOutOfRangeException()
        {
            char[] columns = { 'K', 'L', '0', '/' };
            int[] rows = { 11, -1};

            for (int column = 0; column < columns.Length; column++)
            {
                for (int row = 0; row < rows.Length; row++)
                {
                    string inputString = columns[column] + rows[row].ToString();
                    Assert.Throws<ArgumentOutOfRangeException>(() => engine.ParseInput(inputString));
                }
            }
        }

        [Fact]
        public void ParseInput_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => engine.ParseInput(""));
            Assert.Throws<ArgumentException>(() => engine.ParseInput(" "));
            Assert.Throws<ArgumentException>(() => engine.ParseInput("D"));
        }

        [Fact]
        public void PutShipOutOfRange_Fail()
        {
            var scenarioOne = new { x = Grid.Size, y = Grid.Size };
            var scenarioTwo = new { x = -1, y = -1 };
            int[] shipSizes = { 3, 4 };
            ShipOrientation[] orientations = { ShipOrientation.Vertical, ShipOrientation.Horizontal };
            const int inRange = 1; // random value that is in range

            foreach (var shipSize in shipSizes)
            {
                foreach (var orientation in orientations)
                {
                    var resultOne = engine.PlaceShip(playerOne, new Ship() { X = scenarioOne.x, Y = inRange, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);

                    resultOne = engine.PlaceShip(playerOne, new Ship() { X = inRange, Y = scenarioOne.y, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);

                    resultOne = engine.PlaceShip(playerOne, new Ship() { X = scenarioOne.x, Y = scenarioOne.y, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);

                    resultOne = engine.PlaceShip(playerOne, new Ship() { X = scenarioTwo.x, Y = inRange, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);

                    resultOne = engine.PlaceShip(playerOne, new Ship() { X = inRange, Y = scenarioTwo.y, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);

                    resultOne = engine.PlaceShip(playerOne, new Ship() { X = scenarioTwo.x, Y = scenarioTwo.y, Size = shipSize, Orientation = orientation });
                    Assert.False(resultOne);
                }
            }
        }

        [Fact]
        public void PutShipPartiallyOutOfRange_Fail()
        {
            var firstShip = new Ship()
            {
                X = 7,
                Y = 7,
                Size = 4,
                Orientation = ShipOrientation.Horizontal
            };
            
            var secondShip = new Ship()
            {
                X = 7,
                Y = 7,
                Size = 4,
                Orientation = ShipOrientation.Vertical
            };


            var result = engine.IsPlacingShipPossible(playerOne, firstShip);
            Assert.False(result);
            result = engine.IsPlacingShipPossible(playerOne, secondShip);
            Assert.False(result);
        }

        [Fact]
        public void PutOneShipOnAnother_Fail()
        {
            var firstShip = new Ship
            { 
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Vertical,
            };
            var secondShip = new Ship
            {
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };

            var result = engine.PlaceShip(playerOne, firstShip);
            Assert.True(result);
            result = engine.PlaceShip(playerOne, secondShip);
            Assert.False(result);
        }

        [Fact]
        public void PutTwoShipsNextToEachOther_Fail()
        {
            var firstShip = new Ship
            {
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };
            var secondShip = new Ship
            {
                X = 4,
                Y = 4,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };
            var thirdShip = new Ship
            {
                X = 4,
                Y = 8,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };
            var fourthShip = new Ship
            {
                X = 5,
                Y = 9,
                Size = 3,
                Orientation = ShipOrientation.Horizontal,
            };

            var result = engine.PlaceShip(playerOne, firstShip);
            Assert.True(result);
            result = engine.PlaceShip(playerOne, secondShip);
            Assert.False(result);

            result = engine.PlaceShip(playerOne, thirdShip);
            Assert.True(result);
            result = engine.PlaceShip(playerOne, fourthShip);
            Assert.False(result);
        }

        [Fact]
        public void PutShipNextToEachOtherWhenFirstCellsAreClose_Fail()
        {
            var ship = new Ship
            {
                X = 5,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };
            int[] yRange = { 4, 6 };
            ShipOrientation[] orientations = { ShipOrientation.Horizontal, ShipOrientation.Vertical };

            var result = engine.PlaceShip(playerOne, ship);
            Assert.True(result);

            foreach (var orientation in orientations)
            {
                foreach (var y in yRange)
                {
                    for (int x = ship.X - 1; x <= ship.Size + 1; ++x)
                    {
                        var newShip = new Ship
                        {
                            X = x,
                            Y = y,
                            Size = 4,
                            Orientation = orientation,
                        };
                        result = engine.PlaceShip(playerOne, newShip);
                        Assert.False(result);
                    }
                }
            }
        }

        [Fact]
        public void PutShipNextToEachOtherWhenLastCellsAreClose_Fail()
        {
            var ship = new Ship
            {
                X = 0,
                Y = 5,
                Size = 4,
                Orientation = ShipOrientation.Horizontal,
            };

            Ship[] shipsToCheck =
            {
                new Ship()
                { 
                    X = 3,
                    Y = 4,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
                new Ship()
                {
                    X = 3,
                    Y = 5,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
                new Ship()
                {
                    X = 3,
                    Y = 6,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
                new Ship()
                {
                    X = 4,
                    Y = 4,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
                new Ship()
                {
                    X = 4,
                    Y = 5,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
                new Ship()
                {
                    X = 4,
                    Y = 6,
                    Size = 4,
                    Orientation = ShipOrientation.Horizontal,
                },
            };

            int[] yRange = { 4, 6 };
            ShipOrientation[] orientations = { ShipOrientation.Horizontal, ShipOrientation.Vertical };

            var result = engine.PlaceShip(playerOne, ship);
            Assert.True(result);

            foreach (var orientation in orientations)
            {
                foreach (var shipToCheck in shipsToCheck)
                {
                    ship.Orientation = orientation;
                    result = engine.PlaceShip(playerOne, ship);
                    Assert.False(result);
                }
            }
        }

        [Fact]
        public void ShotShip_Fail()
        {
            int[] emptyCells = Enumerable.Range(0, 99).ToArray();
            int[] shipCells = { 55, 56, 57, 58 };
            int[] filteredArray = emptyCells.Except(shipCells).ToArray();
            
            for (int shotPosition = 0; shotPosition < filteredArray.Length; shotPosition++)
            {
                var position = new
                {
                    x = filteredArray[shotPosition] % Grid.Size,
                    y = filteredArray[shotPosition] / Grid.Size
                };
                Assert.False(playerOne.ShotShip(position.x, position.y));
            }
        }
    }
}