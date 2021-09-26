using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
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

        private CraftingResources _currentCraftingResource;
        private EShopRequestStates _requestState;

        public void UpdateData(CraftingResources resource, EShopRequestStates state)
        {
            _currentCraftingResource = resource;
            _requestState = state;
        }

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.ResourceType.ToString();
            numberText.text = "INVENTORY: " + ItemDetails.InventoryNumber;
			iconImage.sprite = ItemDetails.Icon;
        }
        
        protected override bool Selected()
        {
            var toReturn = _currentCraftingResource == ItemDetails.ResourceType &&
                           _requestState == EShopRequestStates.MakeRequest;

            return toReturn;
        }
    }
}
