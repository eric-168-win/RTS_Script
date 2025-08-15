using System.Collections;
using System.Collections.Generic;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using UnityEngine;
using UnityEngine.AI;


namespace RTS_LEARN.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public int QueueSize => buildingQueue.Count;
        public AbstractUnitSO[] Queue => buildingQueue.ToArray();
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public AbstractUnitSO BuildingUnit { get; private set; }
        [field: SerializeField] public MeshRenderer MainRenderer { get; private set; }
        [field: SerializeField]
        public BuildingProgress Progress { get; private set; } = new(
            BuildingProgress.BuildingState.Destroyed, 0, 0
        );
        [field: SerializeField] public BuildingSO BuildingSO { get; private set; }


        [SerializeField] private Material primaryMaterial;
        [SerializeField] private NavMeshObstacle navMeshObstacle;

        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;

        private List<AbstractUnitSO> buildingQueue = new(MAX_QUEUE_SIZE);
        private const int MAX_QUEUE_SIZE = 5;
        private IBuildingBuilder unitBuildingThis;


        private void Awake()
        {
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
            Bus<BuildingSpawnEvent>.Raise(Owner, new BuildingSpawnEvent(this));
        }

        public void BuildUnit(AbstractUnitSO unit)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE)
            {
                return;
            }
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(-unit.Cost.Minerals, unit.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(-unit.Cost.Gas, unit.Cost.GasSO));


            buildingQueue.Add(unit);
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

            AbstractUnitSO unitSO = buildingQueue[index];
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(unitSO.Cost.Minerals, unitSO.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(unitSO.Cost.Gas, unitSO.Cost.GasSO));
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

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent[Owner] -= HandleUnitDeath;
        }


        private IEnumerator DoBuildUnit()
        {
            while (buildingQueue.Count > 0)
            {
                BuildingUnit = buildingQueue[0];
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());

                yield return new WaitForSeconds(BuildingUnit.BuildTime);

                GameObject instance = Instantiate(BuildingUnit.Prefab, transform.position, Quaternion.identity);
                if (instance.TryGetComponent(out AbstractCommandable commandable))
                {
                    commandable.Owner = Owner;
                }
                buildingQueue.RemoveAt(0);
            }
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }
    }


}


