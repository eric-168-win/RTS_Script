using UnityEngine;

namespace RTS_LEARN.Units
{
    public interface IMoveable
    {
        void MoveTo(Vector3 position);
        void StopMoving();
    }
}