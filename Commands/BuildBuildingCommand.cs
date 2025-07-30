using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : ActionBase
    {
        [field: SerializeField] public BuildingSO Building { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is IBuildingBuilder;
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;
            builder.Build(Building, context.Hit.point);
        }
    }

}