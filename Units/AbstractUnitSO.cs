using RTS_LEARN.TechTree;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public abstract class AbstractUnitSO : UnlockableSO
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}