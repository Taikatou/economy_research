using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
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
            nameLabel.text = ItemDetails.item.itemName;
            priceText.text = ItemDetails.shopDetails.price.ToString();
            stockText.text = ItemDetails.shopDetails.stock.ToString();
        }
    }
}
