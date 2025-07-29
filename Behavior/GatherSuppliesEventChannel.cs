using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;


namespace RTS_LEARN.Behavior
{

#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/GatherSuppliesEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "GatherSuppliesEventChannel", message: "[Self] gathers [Amount] [Supplies] .", category: "Events", id: "85c01280942f19592fcdfca27452e9e6")]
    public sealed partial class GatherSuppliesEventChannel : EventChannel<GameObject, int, SupplySO> { }

}
