using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request.Buttons
{
    public struct AdventurerCraftingResourceRequest
    {
        public CraftingResourceRequest Request;
        public int CurrentNumber;
    }
    public class AdventurerCurrentRequestButton : SampleButton<AdventurerCraftingResourceRequest>
    {
        public Text nameLabel;
        public Text stockText;
        public Text price;
		public Image iconImage;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Request.Resource.ToString();
            stockText.text = ItemDetails.CurrentNumber + "/" + ItemDetails.Request.Number;
            price.text = ItemDetails.Request.Price.ToString();
			iconImage.sprite = ItemDetails.Request.Icon;
		}
    }
}
