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

        private ECraftingResources _currentECraftingResource;

        private bool _correctState;
        
        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Resource.ToString();
			iconImage.sprite = ItemDetails.Icon;
			stockText.text = "x" + ItemDetails.Number;
            price.text = ItemDetails.Price.ToString();
        }

        public void UpdateData(ECraftingResources resource)
        {
            UpdateData(resource);
        }

        public void UpdateData(ECraftingResources resource, bool correctState)
        {
            _currentECraftingResource = resource;
            _correctState = correctState;
        }

        protected override bool Selected()
        {
            var toReturn = _currentECraftingResource == ItemDetails.Resource &&
                           _correctState;

            return toReturn;
        }
    }
}
