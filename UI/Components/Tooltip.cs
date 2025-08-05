using TMPro;
using UnityEngine;

namespace RTS_LEARN.UI.Components
{
    public class Tooltip : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [field: SerializeField][Range(0, 1)] public float HoverDelay { get; private set; } = 0.5f;
        [SerializeField] private TextMeshProUGUI text;

        // public void SetText(string text) => this.text.SetText(text);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void Awake() => RectTransform = GetComponent<RectTransform>();

        public void SetText(string text)
        {
            this.text.SetText(text);
            Vector2 preferredSize = this.text.GetPreferredValues();
            RectTransform.sizeDelta = new Vector2(preferredSize.x + 50, RectTransform.sizeDelta.y);
        }

    }
}
