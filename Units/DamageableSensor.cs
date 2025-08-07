using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RTS_LEARN.Units
{
    [RequireComponent(typeof(Collider))]
    public class DamageableSensor : MonoBehaviour
    {
        private HashSet<IDamageable> damageables = new();
        public List<IDamageable> Damageables => damageables.ToList();

        public delegate void UnitDetectionEvent(IDamageable damageable);
        public UnitDetectionEvent OnUnitEnter;
        public UnitDetectionEvent OnUnitExit;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageables.Add(damageable);
                OnUnitEnter?.Invoke(damageable);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageables.Remove(damageable);
                OnUnitExit?.Invoke(damageable);
            }
        }
    }

}
