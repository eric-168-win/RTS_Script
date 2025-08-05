
using RTS_LEARN.Commands;
using RTS_LEARN.Units;
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

        private bool isActive;
        private RectTransform rectTransform;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();
            Disable();
        }

        public void EnableFor(BaseCommand command, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            SetIcon(command.Icon);
            button.interactable = !command.IsLocked(new CommandContext());
            button.onClick.AddListener(onClick);
            isActive = true;

            if (tooltip != null)
            {
                tooltip.SetText(GetTooltipText(command));
            }
        }

        public void Disable()
        {
            SetIcon(null);
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            isActive = false;
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
            if (isActive)
            {
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
            }
        }

        private void ShowTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Show();
                tooltip.RectTransform.position = new Vector2(
                    rectTransform.position.x + rectTransform.rect.width / 2f,
                    rectTransform.position.y + rectTransform.rect.height / 2f
                );
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData _)
        {
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
            //cancel all pending invocations
            // CancelInvoke(nameof(ShowTooltip));
        }

        private string GetTooltipText(BaseCommand command)
        {
            string tooltipText = command.Name + "\n";

            SupplyCostSO supplyCost = null;
            if (command is BuildUnitCommand unitCommand)
            {
                supplyCost = unitCommand.Unit.Cost;
            }
            else if (command is BuildBuildingCommand buildingCommand)
            {
                supplyCost = buildingCommand.BuildingSO.Cost;
            }

            if (supplyCost != null)
            {
                if (supplyCost.Minerals > 0)
                {
                    tooltipText += $"{supplyCost.Minerals} Minerals.";
                }
                if (supplyCost.Gas > 0)
                {
                    tooltipText += $"{supplyCost.Gas} Gas.";
                }
            }

            return tooltipText;
        }

    }
}
