using RTS_LEARN.Player;
using RTS_LEARN.TechTree;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Build Unit Action", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuildUnitCommand : BaseCommand, IUnlockableCommand
    {
        [field: SerializeField] public AbstractUnitSO Unit { get; private set; }


        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding && HasEnoughSupplies(context);
        }

        public override void Handle(CommandContext context)
        {
            if (!HasEnoughSupplies(context)) return;

            BaseBuilding building = (BaseBuilding)context.Commandable;
            building.BuildUnlockable(Unit);
        }

        public override bool IsLocked(CommandContext context) =>
            !HasEnoughSupplies(context) || !Unit.TechTree.IsUnlocked(context.Owner, Unit);

        public UnlockableSO[] GetUnmetDependencies(Owner owner)
        {
            return Unit.TechTree.GetUnmetDependencies(owner, Unit);
        }

        private bool HasEnoughSupplies(CommandContext context)
        {
            return Unit.Cost.Minerals <= Supplies.Minerals[context.Owner] && Unit.Cost.Gas <= Supplies.Gas[context.Owner];
        }

    }
}