using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class ShopInventoryScrollButton : SampleButton<ShopItem>
    {
        public Text nameLabel;
        public Text priceText;
        public Text stockText;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Item.itemName;
            priceText.text = ItemDetails.ShopDetails.price.ToString();
            stockText.text = ItemDetails.ShopDetails.stock.ToString();
        }
    }
}
