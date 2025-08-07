using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RTS_LEARN.Units
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageableSensor : MonoBehaviour
    {
        private new SphereCollider collider;//new Keyword because MonoBehaviour has a collider property
        private HashSet<IDamageable> damageables = new();
        public List<IDamageable> Damageables => damageables.ToList();

        public delegate void UnitDetectionEvent(IDamageable damageable);
        // public UnitDetectionEvent OnUnitEnter;
        // public UnitDetectionEvent OnUnitExit;
        public event UnitDetectionEvent OnUnitEnter;
        public event UnitDetectionEvent OnUnitExit;


        private void Awake()
        {
            collider = GetComponent<SphereCollider>();
        }


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

        public void SetupFrom(AttackConfigSO attackConfig)//invoke by AbstractUnit.cs Start()
        {
            collider.radius = attackConfig.AttackRange;
        }

    }

}
