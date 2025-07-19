using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Build Unit Action", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuidUnitCommand : ActionBase
    {
        [field: SerializeField] public UnitSO Unit { get; private set; }


        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding;
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = (BaseBuilding)context.Commandable;
            building.BuildUnit(Unit);
        }
    }
}