using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.GameEconomy;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
		public GetCurrentShopAgent currentShopAgent;
		public ShopChooserSubSystem shopChooserSubSystem;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;

        protected override List<ShopItem> GetItemList()
        {
			var toReturn = new List<ShopItem>();
			foreach(ShopAgent shopAgent in currentShopAgent.GetAgents)
			{
				var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
				foreach (var item in items)
				{
					toReturn.Add(new ShopItem
					{
						Seller = shopAgent,
						Item = item,
						Price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent, item.itemDetails),
						Number = shopSubSystem.system.shopSubSubSystem.GetNumber(shopAgent, item.itemDetails)
					});
				}
			}

            return toReturn;
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.PurchaseItem, null, item);
		}
    }
}
