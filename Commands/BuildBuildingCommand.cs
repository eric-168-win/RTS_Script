using System.Linq;
using RTS_LEARN.Player;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : BaseCommand
    {
        [field: SerializeField] public BuildingSO BuildingSO { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            if (context.Commandable is not IBuildingBuilder) return false;

            if (context.Hit.collider != null)
            {
                // Check if the collider is a valid building and if it matches the building type
                // or if it is in a state that allows resuming construction

                return context.Hit.collider.TryGetComponent(out BaseBuilding building)
                    && BuildingSO == building.BuildingSO
                    && (building.Progress.State == BuildingProgress.BuildingState.Paused
                        || building.Progress.State == BuildingProgress.BuildingState.Destroyed
                    );
            }

            return HasEnoughSupplies() && AllRestrictionsPass(context.Hit.point);
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;
            if (context.Hit.collider != null
                && context.Hit.collider.TryGetComponent(out BaseBuilding building))
            {
                builder.ResumeBuilding(building);
            }
            else if (HasEnoughSupplies() && AllRestrictionsPass(context.Hit.point))
            {
                builder.Build(BuildingSO, context.Hit.point);
            }
        }

        public override bool IsLocked(CommandContext context) => !HasEnoughSupplies();

        private bool HasEnoughSupplies() => BuildingSO.Cost.Minerals <= Supplies.Minerals && BuildingSO.Cost.Gas <= Supplies.Gas;


    }
}