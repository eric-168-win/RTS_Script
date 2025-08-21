using RTS_LEARN.TechTree;
using RTS_LEARN.Units;

namespace RTS_LEARN.Commands
{
    public interface IUnlockableCommand
    {
        public UnlockableSO[] GetUnmetDependencies(Owner owner);
    }
}
