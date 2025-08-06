using System.Collections.Generic;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Utilities
{
    public struct ClosestCommandPostComparer : IComparer<BaseBuilding>
    {
        private Vector3 targetPosition;

        public ClosestCommandPostComparer(Vector3 position)
        {
            targetPosition = position;
        }

        public int Compare(BaseBuilding x, BaseBuilding y)
        {
            return (x.transform.position - targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - targetPosition).sqrMagnitude);
        }
    }
}