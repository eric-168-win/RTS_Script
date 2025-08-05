using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public abstract class AbstractUnitSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; } = "New Unit";
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float BuildTime { get; private set; } = 5;
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public SupplyCostSO Cost { get; private set; }
    }
}