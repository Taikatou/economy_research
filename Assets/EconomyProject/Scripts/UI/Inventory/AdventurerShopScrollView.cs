using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
		public GetCurrentShopAgent currentShopAgent;
		public ShopChooserSubSystem shopChooserSubSystem;

		public AdventurerShopSystemBehaviour adventurerShopSystem;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;

        protected override List<ShopItem> GetItemList()
        {
	        var counter = 0;
			var toReturn = new List<ShopItem>();
			foreach(var shopAgent in currentShopAgent.GetAgents)
			{
				var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
				foreach (var item in items)
				{
					toReturn.Add(new ShopItem
					{
						Seller = shopAgent,
						Item = item,
						Price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent, item.itemDetails),
						Number = shopSubSystem.system.shopSubSubSystem.GetNumber(shopAgent, item.itemDetails),
						Index = counter
					});
					
					counter++;
				}
			}

            return toReturn;
        }

        protected override void Update()
        {
	        base.Update();

	        foreach (var button in buttons)
	        {
		        var index = adventurerShopSystem.system.adventurerShopSubSystem.GetCurrentLocation(
			        currentAdventurerAgent.CurrentAgent);
		        button.UpdateData(index, Selected);
	        }
        }

        private bool Selected => adventurerShopSystem.system.GetChoice(currentAdventurerAgent.CurrentAgent) 
                                 == ESelectionState.PurchaseItem;

        public override void SelectItem(ShopItem item, int number = 1)
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.PurchaseItem, 
				null, item);
		}
    }
}
