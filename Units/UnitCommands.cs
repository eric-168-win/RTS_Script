using System;
using Unity.Behavior;

namespace RTS_LEARN.Units
{
    [BlackboardEnum]
    public enum UnitCommands
    {
        Stop,
        Move,
        Gather,
        ReturnSupplies,
        BuildBuilding,
        ResumeBuilding,
    }
}
