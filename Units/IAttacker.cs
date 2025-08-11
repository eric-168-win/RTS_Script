using System.Numerics;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public interface IAttacker
    {
        public Transform Transform { get; }
        public void Attack(IDamageable damageable);
        public void Attack(UnityEngine.Vector3 location);
    }
}
