using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.TechTree
{
    [CreateAssetMenu(fileName = "Tech Tree", menuName = "Tech Tree/Tech Tree", order = 1)]
    public class TechTreeSO : ScriptableObject
    {
        [SerializeField] private List<UnlockableSO> allUnlockables = new();
        public IEnumerable<UnlockableSO> AllUnlockables => allUnlockables.ToList();

        private Dictionary<Owner, Dictionary<UnlockableSO, Dependency>> techTrees;

        private void OnEnable()
        {
            if (techTrees == null)
            {
                BuildTechTrees();
            }
        }

        private void OnDisable()
        {
            techTrees = null;
        }

        private void BuildTechTrees()
        {
            techTrees = new Dictionary<Owner, Dictionary<UnlockableSO, Dependency>>();
            Debug.Log($"Building Tech Tree {name}");

            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                Debug.Log($"Adding {owner} to Tech Trees Dictionary");
                techTrees.Add(owner, new Dictionary<UnlockableSO, Dependency>());

                foreach (UnlockableSO unlockableSO in allUnlockables)
                {
                    techTrees[owner].Add(unlockableSO, new Dependency(unlockableSO));
                    Debug.Log($"Configuring {unlockableSO}'s {unlockableSO.UnlockRequirements.Count()} dependencies");
                }
            }
        }

        private readonly struct Dependency
        {
            public HashSet<UnlockableSO> Dependencies { get; }

            public Dependency(UnlockableSO unlockable)
            {
                Dependencies = new HashSet<UnlockableSO>(unlockable.UnlockRequirements);
            }
        }
    }
}
