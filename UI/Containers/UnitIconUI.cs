using RTS_LEARN.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTS_LEARN.UI.Containers
{

    public class UnitIconUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI healthText;

        private AbstractCommandable commandable;

        private const string HEALTH_TEXT_FORMAT = "{0} / {1}";

        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, commandable.CurrentHealth, commandable.MaxHealth));
            icon.sprite = commandable.UnitSO.Icon;

            this.commandable = commandable;
            commandable.OnHealthUpdated -= OnHealthUpdated;//ensure no duplicate handlers
            commandable.OnHealthUpdated += OnHealthUpdated;
        }

        public void Disable()
        {
            gameObject.SetActive(false);

            if (commandable != null)
            {
                commandable.OnHealthUpdated -= OnHealthUpdated;
                commandable = null;
            }
        }

        private void OnHealthUpdated(AbstractCommandable commandable, int _, int currentHealth)
        {
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, currentHealth, commandable.MaxHealth));
        }
    }
}
