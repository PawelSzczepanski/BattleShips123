using BattleShip123Core.Interfaces;
using BattleShip123Core.Model;
using static BattleShip123Core.Interfaces.IEngine;

namespace BattleShip123Engine.Models
{
    public class ComputeConstraints : IComputeConstraints
    {
        public ComputeConstraints(int gridSize)
        {
            this.gridSize = gridSize;
        }

        private readonly int gridSize;

        private bool IsBorderOnLeft(int x)
        {
            if (x - 1 < 0)
            {
                return true;
            }
            return false;
        }

        private bool IsBorderOnRight(int x)
        {
            if (x + 1 >= gridSize)
            {
                return true;
            }
            return false;
        }

        private bool IsBorderOnTop(int y)
        {
            if (y - 1 < 0)
            {
                return true;
            }
            return false;
        }

        private bool IsBorderOnBottom(int y)
        {
            if (y + 1 >= gridSize)
            {
                return true;
            }
            return false;
        }

        public Constraint GetLoopConstraints(Ship ship)
        {
            if (ship.Orientation == ShipOrientation.Horizontal)
            {
                int lastShipCell = ship.X + ship.Size;
                return new Constraint()
                {
                    xLoopStartPos = IsBorderOnLeft(ship.X) ? ship.X : ship.X - 1,
                    yLoopStartPos = IsBorderOnTop(ship.Y) ? ship.Y : ship.Y - 1,
                    xStopCondition = IsBorderOnRight(lastShipCell) ? lastShipCell : lastShipCell + 1,
                    yStopCondition = IsBorderOnBottom(ship.Y + 1) ? ship.Y + 1 : ship.Y + 2
                };
            }
            else
            {
                int lastShipCell = ship.Y + ship.Size;
                return new Constraint()
                {
                    xLoopStartPos = IsBorderOnLeft(ship.X) ? ship.X : ship.X - 1,
                    yLoopStartPos = IsBorderOnTop(ship.Y) ? ship.Y : ship.Y - 1,
                    xStopCondition = IsBorderOnRight(ship.X) ? ship.X + 1 : ship.X + 2,
                    yStopCondition = IsBorderOnBottom(lastShipCell) ? lastShipCell : lastShipCell + 1
                };
            }
        }
    }
}
