using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerInventoryScrollButton : SampleButton<ItemUi>
    {
        public Text nameLabel;
        public Text stockText;
		public Image image;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.ItemDetails.itemName;
            stockText.text = ItemDetails.Number.ToString();
			image.sprite = ItemDetails.ItemDetails.icon;
        }
    }
}
