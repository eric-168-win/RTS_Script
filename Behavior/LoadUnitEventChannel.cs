using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace RTS_LEARN.Behavior
{

#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Load Unit Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Load Unit Event Channel", message: "[Self] loads [TargetGameObject] into itself.", category: "Events", id: "5edd2a9d9b1681db1eaf1ca738209de1")]
    public sealed partial class LoadUnitEventChannel : EventChannel<GameObject, GameObject> { }

}
