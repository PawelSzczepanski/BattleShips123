using BattleShip123Core.Interfaces;
using BattleShip123Core.Model;

namespace BattleShip123View
{
    public class View : IView
    {
        public View()
        {
        }

        /// <summary>
        /// Draw ships for a given player
        /// </summary>
        /// <param name="player">Player that board will be drawn</param>
        public void DrawShips(Player player)
        {
            Console.Clear();
            Console.WriteLine($"           {player.Name}        ");
            Console.WriteLine("    A  B  C  D  E  F  G  H  I  J");

            for (int y = 0; y < Grid.Size; y++)
            {
                string rowNumber = (y + 1).ToString("00");
                Console.Write($"{rowNumber} |");
                for (int x = 0; x < Grid.Size; x++)
                {
                    int coordinate = y * Grid.Size + x;
                    Console.Write($"{player.Grid[coordinate]}  ");
                }
                Console.WriteLine();
            }
        }
    }
}
