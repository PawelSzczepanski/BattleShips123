namespace BattleShip123Core.Model
{
    public class Player
    {
        public Player(string name)
        {
            Reset();
            Name = name;
        }

        public string Name { get; set; }
        public int[] ships { get; set; } = { 5, 4, 4 };
        public Grid Grid { get; set; } = new Grid();
        public int sumOfShipCells { get; set; }

        /// <summary>
        /// Check if there was ship under specified cell, if yes return true
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>True if ship was shot</returns>
        public bool ShotShip(int x, int y)
        {
            int coordinate = x + y * Grid.Size;
            if (Grid[coordinate] != Grid.ShipCell)
            {
                Grid[coordinate] = Grid.ShotedEmptyCell;
                return false;
            }

            --sumOfShipCells;
            Grid[coordinate] = Grid.ShotedShip;
            return true;
        }

        /// <summary>
        /// Reset board and ships to default.
        /// </summary>
        public void Reset()
        {
            sumOfShipCells = ships.Sum();
            Grid.Reset();
        }
    }
}
