using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public AdventurerShopSystem adventurerShopSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        
        protected override LastUpdate LastUpdated => adventurerShopSystem.agentShopSubSystem;
        
        private ShopAgent ShopAgent => adventurerShopSystem.GetCurrentShop(currentAdventurerAgent.CurrentAgent);

        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = adventurerShopSystem.agentShopSubSystem.GetShopItems(ShopAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = adventurerShopSystem.agentShopSubSystem.GetPrice(ShopAgent, item.itemDetails),
                    Number = adventurerShopSystem.agentShopSubSystem.GetNumber(ShopAgent, item.itemDetails)
                });
            }

            return toReturn;
        }

        public override void SelectItem(ShopItemUi item, int number = 1)
        {
            adventurerShopSystem.agentShopSubSystem.PurchaseItem(ShopAgent, item.Item.itemDetails, 
                currentAdventurerAgent.CurrentAgent.wallet,
                currentAdventurerAgent.CurrentAgent.adventurerInventory.agentInventory);
        }
    }
}
