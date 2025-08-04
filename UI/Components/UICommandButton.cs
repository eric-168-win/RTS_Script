
using RTS_LEARN.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTS_LEARN.UI.Components
{
    [RequireComponent(typeof(Button))]//force to add If not present
    public class UICommandButton : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityAction OnPointerEnter { get; set; }
        public UnityAction OnPointerExit { get; set; }

        [SerializeField] private BaseCommand command;
        [SerializeField] private Image icon;
        [SerializeField] private Tooltip tooltip;

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

            if (tooltip != null)
            {
                tooltip.SetText(command.name);
            }
        }

        public void Disable()
        {
            SetIcon(null);
            button.interactable = false;
            button.onClick.RemoveAllListeners();

            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
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

        void IPointerEnterHandler.OnPointerEnter(PointerEventData _)
        {
            // Invoke("ShowTooltip", 0.5f); //hardcoded string, prone to errors
            Invoke(nameof(ShowTooltip), 0.5f);
        }

        private void ShowTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Show();
            }

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData _)
        {
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();//cancel all pending invocations
            // CancelInvoke(nameof(ShowTooltip));
        }
    }
}
