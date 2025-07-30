using System.Collections;
using RTS_LEARN.UI.Components;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private UIBuildQueueButton[] unitButtons;
        [SerializeField] private ProgressBar progressBar;

        private Coroutine buildCoroutine;
        private BaseBuilding building;

        public void EnableFor(BaseBuilding item)
        {
            progressBar.SetProgress(0);
            gameObject.SetActive(true);
            building = item;
            building.OnQueueUpdated += HandleQueueUpdated;
            SetupUnitButtons();

            buildCoroutine = StartCoroutine(UpdateUnitProgress());
        }

        private void SetupUnitButtons()
        {
            int i = 0;
            for (; i < building.QueueSize; i++)
            {
                int index = i;
                // unitButtons[i].EnableFor(building.Queue[i], CancelThatBuilding(index));
                // unitButtons[i].EnableFor(building.Queue[i], () => building.CancelBuildingUnit(i)); //i is not correct
                unitButtons[i].EnableFor(building.Queue[i], () => building.CancelBuildingUnit(index));
            }
            for (; i < unitButtons.Length; i++)
            {
                unitButtons[i].Disable();
            }
        }

        private void HandleQueueUpdated(AbstractUnitSO[] unitsInQueue)
        {
            if (unitsInQueue.Length == 1 & buildCoroutine == null)
            {
                StartCoroutine(UpdateUnitProgress());
            }
            else if (unitsInQueue.Length == 0)
            {
                progressBar.SetProgress(0);
            }
            SetupUnitButtons();
        }

        public void Disable()
        {
            if (building != null)
            {
                building.OnQueueUpdated -= HandleQueueUpdated;
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

            buildCoroutine = null;
        }
    }
}