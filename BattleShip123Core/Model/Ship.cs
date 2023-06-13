using static BattleShip123Core.Interfaces.IEngine;

namespace BattleShip123Core.Model
{
    public class Ship
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }
        public ShipOrientation Orientation { get; set; }
    }
}
