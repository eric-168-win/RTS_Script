using System.Collections;
using System.Collections.Generic;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.Events;
using RTS_LEARN.TechTree;
using UnityEngine;
using UnityEngine.AI;


namespace RTS_LEARN.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public int QueueSize => buildingQueue.Count;
        public UnlockableSO[] Queue => buildingQueue.ToArray();
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnlockableSO SOBeingBuilt { get; private set; }
        [field: SerializeField] public MeshRenderer MainRenderer { get; private set; }
        [field: SerializeField]
        public BuildingProgress Progress { get; private set; } = new(
            BuildingProgress.BuildingState.Destroyed, 0, 0
        );
        [field: SerializeField] public BuildingSO BuildingSO { get; private set; }


        [SerializeField] private Material primaryMaterial;
        [SerializeField] private NavMeshObstacle navMeshObstacle;

        public delegate void QueueUpdatedEvent(UnlockableSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;

        private List<UnlockableSO> buildingQueue = new(MAX_QUEUE_SIZE);
        private const int MAX_QUEUE_SIZE = 5;
        private IBuildingBuilder unitBuildingThis;

        protected override void Awake()
        {
            base.Awake();

            BuildingSO = UnitSO as BuildingSO;
            MaxHealth = BuildingSO.Health;
        }

        protected override void Start()
        {
            base.Start();

            if (MainRenderer != null)
            {
                MainRenderer.material = primaryMaterial;
                // Debug.Log("0000 -> find 1111 " + BuildingSO.PlacementMaterial.name);
            }
            Progress = new BuildingProgress(BuildingProgress.BuildingState.Completed, Progress.StartTime, 1);
            unitBuildingThis = null;
            Bus<UnitDeathEvent>.OnEvent[Owner] -= HandleUnitDeath;
            Bus<BuildingSpawnEvent>.Raise(Owner, new BuildingSpawnEvent(Owner, this));

            foreach (UpgradeSO upgrade in BuildingSO.Upgrades)
            {
                if (BuildingSO.TechTree.IsResearched(Owner, upgrade))
                {
                    upgrade.Apply(BuildingSO);
                }
            }
        }

        public void BuildUnlockable(UnlockableSO unlockable)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE)
            {
                return;
            }
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -unlockable.Cost.Minerals, unlockable.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -unlockable.Cost.Gas, unlockable.Cost.GasSO));


            buildingQueue.Add(unlockable);
            // Debug.Log("BBB:::QueueSize::  " + QueueSize);
            if (buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnit());
            }
            else
            {
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());
            }
        }

        public void CancelBuildingUnit(int index)
        {
            if (index < 0 || index >= buildingQueue.Count) return;

            UnlockableSO unlockableSO = buildingQueue[index];
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, unlockableSO.Cost.Minerals, unlockableSO.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, unlockableSO.Cost.Gas, unlockableSO.Cost.GasSO));
            buildingQueue.RemoveAt(index);
            if (index == 0)
            {
                StopAllCoroutines();
                if (buildingQueue.Count > 0)
                {
                    StartCoroutine(DoBuildUnit());
                }
                else
                {
                    OnQueueUpdated?.Invoke(buildingQueue.ToArray());
                }
            }
            else
            {
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());
            }
        }

        public void StartBuilding(IBuildingBuilder buildingBuilder)
        {
            Awake();
            unitBuildingThis = buildingBuilder;
            Owner = unitBuildingThis.Owner;
            MainRenderer.material = BuildingSO.PlacementMaterial;
            Debug.Log("On The Ground ::: " + BuildingSO.PlacementMaterial.name);

            Progress = new BuildingProgress(
                BuildingProgress.BuildingState.Building,
                Time.time - BuildingSO.BuildTime * Progress.Progress,
                Progress.Progress
            );

            if (Progress.Progress == 0)
            {
                Heal(1);
            }

            //don't want to have duplicate event handlers
            Bus<UnitDeathEvent>.OnEvent[Owner] -= HandleUnitDeath;
            Bus<UnitDeathEvent>.OnEvent[Owner] += HandleUnitDeath;
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (evt.Unit.TryGetComponent(out IBuildingBuilder buildingBuilder) && buildingBuilder == unitBuildingThis)
            {
                Progress = new BuildingProgress(
                    BuildingProgress.BuildingState.Paused,
                    Progress.StartTime,
                    (Time.time - Progress.StartTime) / BuildingSO.BuildTime
                );

                Bus<UnitDeathEvent>.OnEvent[Owner] -= HandleUnitDeath;
            }
        }

        private IEnumerator DoBuildUnit()
        {
            while (buildingQueue.Count > 0)
            {
                SOBeingBuilt = buildingQueue[0];
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());

                yield return new WaitForSeconds(SOBeingBuilt.BuildTime);

                if (SOBeingBuilt is AbstractUnitSO unitSO)
                {

                    GameObject instance = Instantiate(unitSO.Prefab, transform.position, Quaternion.identity);
                    if (instance.TryGetComponent(out AbstractCommandable commandable))
                    {
                        commandable.Owner = Owner;
                    }
                }
                else if (SOBeingBuilt is UpgradeSO upgrade)
                {
                    Bus<UpgradeResearchedEvent>.Raise(Owner, new UpgradeResearchedEvent(Owner, upgrade));
                }

                buildingQueue.RemoveAt(0);
            }
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<UnitDeathEvent>.OnEvent[Owner] -= HandleUnitDeath;
            Bus<BuildingDeathEvent>.Raise(Owner, new BuildingDeathEvent(Owner, this));
        }
    }
}


