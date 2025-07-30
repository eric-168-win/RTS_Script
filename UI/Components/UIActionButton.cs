
using RTS_LEARN.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RTS_LEARN.UI.Components
{
    [RequireComponent(typeof(Button))]//force to add If not present
    public class UIActionButton : MonoBehaviour, IUIElement<ActionBase, UnityAction>
    {
        [SerializeField] private Image icon;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void EnableFor(ActionBase action, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            SetIcon(action.Icon);
            button.interactable = true;
            button.onClick.AddListener(onClick);
        }

        public void Disable()
        {
            SetIcon(null);
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }

        private void SetIcon(Sprite icon)
        {
            if (icon == null)
            {
                this.icon.enabled = false;
            }
            else
            {
                this.icon.sprite = icon;
                this.icon.enabled = true;
            }

        }
    }
}
