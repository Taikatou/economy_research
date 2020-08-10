using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class ShopInventoryScrollButton : SampleButton<ShopItemUi>
    {
        public Text nameLabel;
        public Text priceText;
        public Text stockText;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Item.itemDetails.itemName;
            priceText.text = ItemDetails.Price.ToString();
            stockText.text = ItemDetails.Number.ToString();
        }
    }
}
