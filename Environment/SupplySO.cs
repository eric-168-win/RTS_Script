using UnityEngine;

[CreateAssetMenu(fileName = "SupplySO", menuName = "Supply/Supply", order = 5)]
public class SupplySO : ScriptableObject
{
    [field: SerializeField] public int MaxAmount { get; private set; } = 10000;
    [field: SerializeField] public int AmountPerGather { get; private set; } = 20;
    [field: SerializeField] public float BaseGatherTime { get; private set; } = 1.5f;
}
