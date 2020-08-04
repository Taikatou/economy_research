using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI.Buttons
{
    public class ShopButton : SampleButton<ShopDetails>
    {
        public Text nameLabel;
        public Image iconImage;
        public Text priceText;
        public Text stockText;

        protected override void SetupButton()
        {
            //nameLabel.text = ItemDetails.ItemName;
            //iconImage.sprite = _item.icon;
            priceText.text = ItemDetails.price.ToString();
            stockText.text = "x" + ItemDetails.stock;
        }
    }
}
