using System.Linq;
using RTS_LEARN.Player;
using RTS_LEARN.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : BaseCommand
    {
        [field: SerializeField] public BuildingSO BuildingSO { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            if (context.Commandable is not IBuildingBuilder buildingBuilder || buildingBuilder.IsBuilding) return false;

            if (context.Hit.collider != null && context.Button.ToString() == "Right")//resumming
            {
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
            if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BaseBuilding building))
            {
                builder.ResumeBuilding(building);
            }
            else if (HasEnoughSupplies() && AllRestrictionsPass(context.Hit.point) && context.Button.ToString() != "Right")
            {
                Debug.Log($"Building {BuildingSO.name} at {context.Hit.point}:::: {context.Button}");
                builder.Build(BuildingSO, context.Hit.point);
            }
        }

        public override bool IsLocked(CommandContext context) => !HasEnoughSupplies();

        private bool HasEnoughSupplies() => BuildingSO.Cost.Minerals <= Supplies.Minerals && BuildingSO.Cost.Gas <= Supplies.Gas;


    }
}