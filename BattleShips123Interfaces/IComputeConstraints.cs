using static BattleShip123Engine.Models.IEngine;

namespace BattleShip123Engine.Models
{
    public interface IComputeConstraints
    {
        Constraint GetLoopConstraints(Ship ship);
    }
}