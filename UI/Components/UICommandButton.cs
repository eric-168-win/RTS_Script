
using RTS_LEARN.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RTS_LEARN.UI.Components
{
    [RequireComponent(typeof(Button))]//force to add If not present
    public class UICommandButton : MonoBehaviour, IUIElement<BaseCommand, UnityAction>
    {
        [SerializeField] private Image icon;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void EnableFor(BaseCommand command, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            SetIcon(command.Icon);
            button.interactable = !command.IsLocked(new CommandContext());
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
