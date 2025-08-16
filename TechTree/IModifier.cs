using RTS_LEARN.Units;

namespace RTS_LEARN.TechTree
{
    public interface IModifier
    {
        public string PropertyPath { get; }
        public void Apply(AbstractUnitSO unit);
    }
}
