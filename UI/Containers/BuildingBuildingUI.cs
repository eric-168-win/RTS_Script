using System;
using System.Collections;
using System.Collections.Generic;
using RTS_LEARN.UI.Components;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {

        private Queue<UnitSO> buildingQueue = new(MAX_QUEUE_SIZE);
        public float CurrentQueueStartTime { get; private set; }
        private const int MAX_QUEUE_SIZE = 5;
        [SerializeField] private ProgressBar progressBar;

        private Coroutine buildCoroutine;
        private BaseBuilding building;

        void Awake()
        {

        }

        public void EnableFor(BaseBuilding item)
        {
            gameObject.SetActive(true);
            building = item;
            building.OnQueueUpdated += HandleQueueUpdated;

            buildCoroutine = StartCoroutine(UpdateUnitProgress());
        }

        private void HandleQueueUpdated(UnitSO[] unitsInQueue)
        {
            if (unitsInQueue.Length == 1 & buildCoroutine == null)
            {
                StartCoroutine(UpdateUnitProgress());
            }
        }

        public void Disable()
        {
            if (building != null)
            {
                building.OnQueueUpdated += HandleQueueUpdated;
            }

            gameObject.SetActive(false);
            building = null;
            buildCoroutine = null;
        }

        private IEnumerator UpdateUnitProgress()
        {
            while (building != null && building.QueueSize > 0)
            {
                float startTime = building.CurrentQueueStartTime;
                float endTime = startTime + building.BuildingUnit.BuildTime;

                float progress = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));

                progressBar.SetProgress(progress);
                yield return null;
            }
        }
    }
}