using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request.Buttons
{
    public class CraftingCurrentRequestButton : SampleButton<ResourceRequest>
    {
        public Text nameLabel;
        public Text stockText;
        public Text price;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Resource.ToString();
            //iconImage.sprite = _item.icon;
            stockText.text = "x" + ItemDetails.Number;
            price.text = ItemDetails.Price.ToString();
        }
    }
}
