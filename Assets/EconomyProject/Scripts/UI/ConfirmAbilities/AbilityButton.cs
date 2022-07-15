using EconomyProject.Scripts.UI.ShopUI.Buttons;
using TurnBased.Scripts.AI;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ConfirmAbilities
{
    public class AbilityButton : SampleButton<AbilityUI>
    {
        public Text text;
        protected override void SetupButton()
        {
            text.text = ItemDetails.AbilityName.ToString();
        }

        private bool _selected;
        protected override bool Selected()
        {
            return _selected;
        }

        public void UpdateButton(EAttackOptions option)
        {
            _selected = option == ItemDetails.AbilityName;
        }
    }
}
