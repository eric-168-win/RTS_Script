using UnityEngine;

namespace RTS_LEARN.UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Vector2 padding = new Vector2(9, 8);
        [SerializeField] private RectTransform mask;
        private RectTransform maskParentRectTransform;

        //TODO: remove progress and update, they are just for debugging!
        // [SerializeField][Range(0, 1)] private float progress;
        // public void Update()
        // {
        //     SetProgress(progress);
        // }

        private void Awake()
        {
            if (mask == null)
            {
                Debug.LogError($"Progress bar {name} is missing mask! This progress bar will not work");
                return;
            }

            maskParentRectTransform = mask.parent.GetComponent<RectTransform>();
        }

        public void SetProgress(float progress)
        {
            Vector2 parentSize = maskParentRectTransform.sizeDelta;
            Vector2 targetSize = parentSize - padding * 2;

            targetSize.x *= Mathf.Clamp01(progress);

            mask.offsetMin = padding;//starting from the top left corner
            mask.offsetMax = new Vector2(padding.x + targetSize.x - parentSize.x, -padding.y);


        }

        // public void SetProgress(float progress)
        // {
        //     Vector2 parentSize = maskParentRectTransform.sizeDelta;
        //     Vector2 targetSize = parentSize;

        //     targetSize.x *= Mathf.Clamp01(progress);

        //     mask.offsetMin = Vector2.zero;//starting from the top left corner
        //     mask.offsetMax = targetSize - parentSize;
        // }

    }
}