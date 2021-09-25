using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request.Buttons
{
    public class CraftingCurrentRequestButton : SampleButton<CraftingResourceRequest>
    {
        public Text nameLabel;
        public Text stockText;
        public Text price;
		public Image iconImage;

        private CraftingResources _currentCraftingResource;
        private EShopRequestStates _requestState;
        
        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Resource.ToString();
			iconImage.sprite = ItemDetails.Icon;
			stockText.text = "x" + ItemDetails.Number;
            price.text = ItemDetails.Price.ToString();
        }

        public void UpdateData(CraftingResources resource, EShopRequestStates state)
        {
            _currentCraftingResource = resource;
            _requestState = state;
        }
        
        protected override bool Selected()
        {
            var toReturn = _currentCraftingResource == ItemDetails.Resource &&
                           _requestState == EShopRequestStates.ChangePrice;

            return toReturn;
        }
    }
}
