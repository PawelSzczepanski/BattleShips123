using BattleShip123Core.Model;

namespace BattleShip123Core.Interfaces
{
    public interface IEngine
    {
        enum ShipOrientation { Horizontal, Vertical };
        void NewGame();
        void StartGame();
        void PlaceShips(Player player);
        bool PlaceShip(Player player, Ship ship);
        (int, int) ParseInput(string inputString);
    }
}
