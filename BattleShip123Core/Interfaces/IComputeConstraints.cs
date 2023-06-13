using BattleShip123Core.Model;

namespace BattleShip123Core.Interfaces
{
    public interface IComputeConstraints
    {
        Constraint GetLoopConstraints(Ship ship);
    }
}