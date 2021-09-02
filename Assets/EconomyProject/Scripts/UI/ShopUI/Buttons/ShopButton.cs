using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.Inventory;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI.Buttons
{
    public class ShopButton : SampleButton<ShopItem>
    {
        public Text priceText;
        public Text stockText;

        protected override void SetupButton()
        {
            //nameLabel.text = ItemDetails.ItemName;
            //iconImage.sprite = _item.icon;
            priceText.text = ItemDetails.Price.ToString();
            stockText.text = "x" + ItemDetails.Number;
        }
    }
}
