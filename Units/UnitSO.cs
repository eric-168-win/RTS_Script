using UnityEngine;

namespace RTS_LEARN.Units
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "Units/Unit")]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float BuildTime { get; private set; } = 5;
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}