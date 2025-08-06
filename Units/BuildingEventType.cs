using Unity.Behavior;

namespace RTS_LEARN.Units
{
    [BlackboardEnum]
    public enum BuildingEventType
    {
        ArrivedAt,
        Begin,
        Cancel, //regret and then get refund
        Abort, //when get to the building, but cannot processed that building (e.g. singleUnitCommand but choose more than one unit)
        Completed,
    }
}