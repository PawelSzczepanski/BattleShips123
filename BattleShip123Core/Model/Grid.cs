namespace BattleShip123Core.Model
{
    public class Grid
    {
        public Grid()
        {
            Reset();
        }
        public const char EmptyCell = ' ';
        public const char ShipCell = 'O';
        public const char ShotedShip = 'X';
        public const char ShotedEmptyCell = '-';
        public const int Size = 10;
        public char[] GameBoard { get; set; } = new char[Size * Size];

        /// <summary>
        /// Set all cells to default value
        /// </summary>
        public void Reset()
        {
            Array.Fill(GameBoard, EmptyCell);
        }

        /// <summary>
        /// Return given cell board
        /// </summary>
        /// <param name="coordinate">Coordinate that we want to check</param>
        /// <returns>Given board cell</returns>
        /// <exception cref="IndexOutOfRangeException">Throw if argument is out of range</exception>
        public char this[int coordinate]
        {
            get
            {
                if (coordinate < 0 || coordinate >= GameBoard.Length)
                    throw new IndexOutOfRangeException("Index out of range");

                return GameBoard[coordinate];
            }

            set
            {
                if (coordinate < 0 || coordinate >= GameBoard.Length)
                    throw new IndexOutOfRangeException("Index out of range");

                GameBoard[coordinate] = value;
            }
        }
    }
}
