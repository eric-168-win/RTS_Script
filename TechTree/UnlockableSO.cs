using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.TechTree
{
    public abstract class UnlockableSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; } = "Unit";
        [field: SerializeField] public float BuildTime { get; private set; } = 5;
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public SupplyCostSO Cost { get; private set; }
        [field: SerializeField] protected List<UnlockableSO> unlockRequirements { get; private set; } = new();

        public IEnumerable<UnlockableSO> UnlockRequirements => unlockRequirements.ToList();
    }
}
