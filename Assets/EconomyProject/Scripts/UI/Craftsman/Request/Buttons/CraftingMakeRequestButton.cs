using EconomyProject.Scripts.UI.Craftsman.Request.ScrollList;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request.Buttons
{
    public class CraftingMakeRequestButton : SampleButton<CraftingResourceUi>
    {
        public Text nameLabel;
        public Image iconImage;
        public Text numberText;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.ResourceType.ToString();
            numberText.text = "INVENTORY: " + ItemDetails.InventoryNumber;
			iconImage.sprite = ItemDetails.Icon;
        }
    }
}
