using RTS_LEARN.Environment;

namespace RTS_LEARN.Units
{
    public class Worker : AbstractUnit //MonoBehaviour, ISelectable, IMoveable
    {
        public void Gather(GatherableSupply supply)
        {
            graphAgent.SetVariableValue("Supply", supply);
            graphAgent.SetVariableValue("TargetGameObject", supply.gameObject);
            graphAgent.SetVariableValue("Command", UnitCommands.Gather);

        }
    }
}