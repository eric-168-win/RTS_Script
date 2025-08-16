using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.TechTree
{
    public abstract class UpgradeSO : UnlockableSO, IModifier
    {
        [field: SerializeField] public string PropertyPath { get; private set; }

        public abstract void Apply(AbstractUnitSO unit);
    }
}
