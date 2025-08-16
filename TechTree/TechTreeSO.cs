using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
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
        public bool IsUnlocked(Owner owner, UnlockableSO unlockable) =>
            techTrees[owner].TryGetValue(unlockable, out Dependency value) && value.IsUnlocked;

        private void OnEnable()
        {
            if (techTrees == null)
            {
                BuildTechTrees();
            }
            Bus<BuildingSpawnEvent>.RegisterForAll(HandleBuildingSpawn);
        }

        private void OnDisable()
        {
            techTrees = null;
            Bus<BuildingSpawnEvent>.UnregisterForAll(HandleBuildingSpawn);
        }

        private void HandleBuildingSpawn(BuildingSpawnEvent evt)
        {
            foreach (KeyValuePair<UnlockableSO, Dependency> keyValuePair in techTrees[evt.Owner])
            {
                keyValuePair.Value.UnlockDependency(evt.Building.BuildingSO);
            }
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
        private readonly Dictionary<UnlockableSO, int> metDependencies;
        private readonly struct Dependency
        {
            public HashSet<UnlockableSO> Dependencies { get; }
            public bool IsUnlocked => Dependencies.Count == metDependencies.Count;
            private readonly Dictionary<UnlockableSO, int> metDependencies;

            public Dependency(UnlockableSO unlockable)
            {
                Dependencies = new HashSet<UnlockableSO>(unlockable.UnlockRequirements);
                metDependencies = new Dictionary<UnlockableSO, int>(Dependencies.Count);
            }

            public void UnlockDependency(UnlockableSO dependency)
            {
                if (Dependencies.Contains(dependency) && !metDependencies.TryAdd(dependency, 1))
                {
                    metDependencies[dependency]++;
                }
            }
        }
    }
}
