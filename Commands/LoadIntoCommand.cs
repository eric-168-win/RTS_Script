using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Load Unit into Transport", menuName = "Units/Commands/Load Unit Into", order = 107)]
    public class LoadIntoCommand : BaseCommand
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is ITransportable
                && context.Hit.collider != null
                && context.Hit.collider.TryGetComponent(out ITransporter _);
        }

        public override void Handle(CommandContext context)
        {
            ITransportable transportable = (ITransportable)context.Commandable;
            ITransporter transporter = context.Hit.collider.GetComponent<ITransporter>();

            transportable.LoadInto(transporter);
        }

        public override bool IsLocked(CommandContext context) => false;
    }
}