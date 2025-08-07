using UnityEngine;

namespace RTS_LEARN.Units
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "Units/Unit")]
    public class UnitSO : AbstractUnitSO
    {
        [field: SerializeField] public AttackConfigSO AttackConfig { get; private set; }
    }
}