using System.Collections;
using RTS_LEARN.UI.Components;
using RTS_LEARN.Units;
using TMPro;
using UnityEngine;

namespace RTS_LEARN.UI.Containers
{
    public class BuildingUnderConstructionUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private TextMeshProUGUI unitName;
        [SerializeField] private ProgressBar progressBar;

        public void EnableFor(BaseBuilding building)
        {
            gameObject.SetActive(true);
            unitName.SetText(building.UnitSO.Name);
            StartCoroutine(AnimateBuildingProgress(building));
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator AnimateBuildingProgress(BaseBuilding building)
        {
            while (enabled && building.Progress.Progress < 1)
            {
                if (building.Progress.State != BuildingProgress.BuildingState.Building)
                {
                    yield return null;
                    continue;
                }

                float startTime = building.Progress.StartTime;
                float endTime = startTime + building.BuildingSO.BuildTime;

                progressBar.SetProgress(Mathf.Clamp01((Time.time - startTime) / (endTime - startTime)));
                yield return null;
            }
        }
    }
}
