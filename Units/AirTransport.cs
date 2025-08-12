using RTS_LEARN.Behavior;
using Unity.Behavior;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public class AirTransport : AbstractUnit
    {
        protected override void Start()
        {
            base.Start();

            if (graphAgent.GetVariable("LoadUnitEventChannel", out BlackboardVariable<LoadUnitEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleLoadUnit;
            }
        }

        private void HandleLoadUnit(GameObject self, GameObject targetGameObject)
        {
            Debug.Log($"Load unit {targetGameObject.name}");
        }

    }

}