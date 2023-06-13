using namespace BattleShip123View;

namespace BattleShip123Engine.Models
{
    public interface IEngine
    {
        enum ShipOrientation { Horizontal, Vertical };
        void NewGame(IView view);
        void StartGame();
        void PlaceShips(Player player);
        bool PlaceShip(Player player, Ship ship);
    }
}
