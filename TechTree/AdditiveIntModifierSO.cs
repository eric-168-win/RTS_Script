using System;
using System.Reflection;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.TechTree
{
    [CreateAssetMenu(fileName = "Additive Int Modifier", menuName = "Tech Tree/Modifiers/Additive Int Modifier", order = 160)]
    public class AdditiveIntModifierSO : UpgradeSO
    {
        [field: SerializeField] public int Amount { get; private set; }

        public override void Apply(AbstractUnitSO unit)
        {
            // assume PropertyPath = "AttackConfig/Damage"
            // ((UnitSO) unit).AttackConfig.Damage += Amount;
            Debug.Log($"{Name} is applying {Amount} to {PropertyPath}.");

            string[] attributes = PropertyPath.Split("/"); // ["AttackConfig", "Damage"]

            Type type = unit.GetType();
            object target = unit;

            for (int i = 0; i < attributes.Length - 1; i++)
            {
                PropertyInfo propertyInfo = type.GetProperty(attributes[i]);

                if (propertyInfo == null)
                {
                    Debug.LogError($"Unable to apply modifier {Name} to attribute {PropertyPath} because" +
                        $" it does not exist on {unit.Name}!");
                    return;
                }

                target = propertyInfo.GetValue(target); // target is now AttackConfigSO!
                type = target.GetType(); // type is now AttackConfigSO instead of AbstractUnitSO!
            }

            PropertyInfo attributeField = type.GetProperty(attributes[attributes.Length - 1]); // Damage!

            if (attributeField == null)
            {
                Debug.LogError($"Unable to apply modifier {Name} to attribute {PropertyPath} because" +
                        $" it does not exist on {unit.Name}!");
                return;
            }

            try
            {
                int currentValue = (int)attributeField.GetValue(target);
                Debug.Log($"Adding {Amount} to {PropertyPath}'s current value of {currentValue}");
                currentValue += Amount;
                attributeField.SetValue(target, currentValue);
                Debug.Log($"Updated value to: {attributeField.GetValue(target)}");
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"Expected {PropertyPath} to be an int, but it wasn't!");
            }
        }

    }
}
