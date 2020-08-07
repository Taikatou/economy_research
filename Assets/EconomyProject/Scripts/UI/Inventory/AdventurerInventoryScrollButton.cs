using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerInventoryScrollButton : SampleButton<InventoryItem>
    {
        public Text nameLabel;
        public Text stockText;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Item.itemName;
            stockText.text = ItemDetails.Number.ToString();
        }
    }
}
