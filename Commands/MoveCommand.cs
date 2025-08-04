using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Move Action", menuName = "Units/Commands/Move", order = 100)]
    public class MoveCommand : BaseCommand
    {
        [SerializeField] private float radiusMultiplied = 3.5f;
        private int unitsOnLayer = 0;
        private int maxUnitsOnLayer = 1; // Example limit, adjust as needed
        private float circleRadius = 0; // Example radius, adjust as needed
        private float radiusOffset = 0; // Example offset, adjust as needed
        public override bool CanHandle(CommandContext context)
        {
            // return context.Commandable is IMoveable;
            return context.Commandable is AbstractUnit;
        }

        public override void Handle(CommandContext context)
        {
            // IMoveable moveable = (IMoveable)context.Commandable;
            AbstractUnit unit = (AbstractUnit)context.Commandable;

            if (context.UnitIndex == 0)
            {
                unitsOnLayer = 0;
                maxUnitsOnLayer = 1;
                circleRadius = 0;
                radiusOffset = 0;
            }

            Vector3 targetPosition = new(
                context.Hit.point.x + circleRadius * Mathf.Cos(radiusOffset * unitsOnLayer),
                context.Hit.point.y,
                context.Hit.point.z + circleRadius * Mathf.Sin(radiusOffset * unitsOnLayer)
            );

            unit.MoveTo(targetPosition);
            unitsOnLayer++;

            if (unitsOnLayer >= maxUnitsOnLayer)
            {
                unitsOnLayer = 0; // Reset the counter for the next layer
                circleRadius += unit.AgentRadius * radiusMultiplied; // Increase the radius for the next layer
                maxUnitsOnLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2)); // Increase the max units on the next layer
                radiusOffset += 2 * Mathf.PI / maxUnitsOnLayer; // Adjust the radial offset for the next layer
            }
        }

        public override bool IsLocked(CommandContext context) => false;
    }

}