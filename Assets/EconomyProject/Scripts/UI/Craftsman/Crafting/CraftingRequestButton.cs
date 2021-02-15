using System.Globalization;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftingInfo
    {
        public CraftingMap craftingMap;
        public CraftingInventory craftingInventory;

        public CraftingInfo(CraftingMap craftingMap, CraftingInventory craftingInventory)
        {
            this.craftingMap = craftingMap;
            this.craftingInventory = craftingInventory;
        }
    }
    public class CraftingRequestButton : SampleButton<CraftingInfo>
    {
        public Text nameLabel;
        public Image iconImage;
        public Text timeToCreated;
        public Text specsText;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.craftingMap.resource.ResultingItemName;
            timeToCreated.text = ItemDetails.craftingMap.resource.timeToCreation.ToString(CultureInfo.InvariantCulture);
			iconImage.sprite = ItemDetails.craftingMap.resource.resultingItem.itemDetails.icon;
			var outputText = "";
            foreach (var requirement in ItemDetails.craftingMap.resource.resourcesRequirements)
            {
                var currentInventory = ItemDetails.craftingInventory.GetResourceNumber(requirement.type);
                outputText += requirement.type + "\t" + requirement.number + "/" + currentInventory + "\n";
            }

            specsText.text = outputText;
        }
    }
}
