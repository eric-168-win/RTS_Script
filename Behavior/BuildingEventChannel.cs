using RTS_LEARN.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace RTS_LEARN.Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Building Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Building Event Channel", message: "[Self] [BuildingEventType] on [BaseBuilding] .", category: "Events", id: "e45e9ea1ca6945d421b9bc4235828594")]
    public sealed partial class BuildingEventChannel : EventChannel<GameObject, BuildingEventType, BaseBuilding> { }
}
