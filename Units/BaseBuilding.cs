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
        [SerializeField] private Material primaryMaterial;
        [SerializeField] private NavMeshObstacle navMeshObstacle;
        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        private BuildingSO buildingSO;

        private List<AbstractUnitSO> buildingQueue = new(MAX_QUEUE_SIZE);
        private const int MAX_QUEUE_SIZE = 5;

        [field: SerializeField]
        public BuildingProgress Progress { get; private set; } = new(
            BuildingProgress.BuildingState.Destroyed, 0, 0
        );
        private IBuildingBuilder unitBuildingThis;

        private void Awake()
        {
            buildingSO = UnitSO as BuildingSO;
        }

        protected override void Start()
        {
            base.Start();

            if (MainRenderer != null)
            {
                MainRenderer.material = primaryMaterial;
            }
            Progress = new BuildingProgress(BuildingProgress.BuildingState.Completed, Progress.StartTime, 1);
            unitBuildingThis = null;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        public void BuildUnit(AbstractUnitSO unit)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE)
                return;

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
            unitBuildingThis = buildingBuilder;
            MainRenderer.material = buildingSO.PlacementMaterial;

            Progress = new BuildingProgress(
                BuildingProgress.BuildingState.Building,
                Time.time - buildingSO.BuildTime * Progress.Progress,
                Progress.Progress
            );

            //don't want to have duplicate event handlers
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (evt.Unit.TryGetComponent(out IBuildingBuilder buildingBuilder) && buildingBuilder == unitBuildingThis)
            {
                Progress = new BuildingProgress(
                    BuildingProgress.BuildingState.Paused,
                    Progress.StartTime,
                    (Time.time - Progress.StartTime) / buildingSO.BuildTime
                );

                Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            }
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }


        private IEnumerator DoBuildUnit()
        {
            while (buildingQueue.Count > 0)
            {
                BuildingUnit = buildingQueue[0];
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());

                yield return new WaitForSeconds(BuildingUnit.BuildTime);

                Instantiate(BuildingUnit.Prefab, transform.position, Quaternion.identity);
                buildingQueue.RemoveAt(0);
            }
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }
    }


}


