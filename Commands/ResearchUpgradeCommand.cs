using RTS_LEARN.Player;
using RTS_LEARN.TechTree;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Research Upgrade", menuName = "Tech Tree/Research Upgrade Command", order = 140)]
    public class ResearchUpgradeCommand : BaseCommand
    {
        [field: SerializeField] public UpgradeSO Upgrade { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding;
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = context.Commandable as BaseBuilding;

            if (HasEnoughSupplies(context))
            {
                building.BuildUnlockable(Upgrade);
            }
        }

        public override bool IsLocked(CommandContext context) =>
            !HasEnoughSupplies(context) || !Upgrade.TechTree.IsUnlocked(context.Owner, Upgrade);

        private bool HasEnoughSupplies(CommandContext context)
        {
            return Upgrade.Cost.Minerals <= Supplies.Minerals[context.Owner]
                && Upgrade.Cost.Gas <= Supplies.Gas[context.Owner];
        }
    }
}
