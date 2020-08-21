using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public AgentShopSubSystem agentShopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        public ShopChooserSubSystem shopChooserSubSystem;
        protected override LastUpdate LastUpdated => agentShopSubSystem;
        
        private ShopAgent ShopAgent => shopChooserSubSystem.GetCurrentShop(currentAdventurerAgent.CurrentAgent);

        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = agentShopSubSystem.GetShopItems(ShopAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = agentShopSubSystem.GetPrice(ShopAgent, item.itemDetails),
                    Number = agentShopSubSystem.GetNumber(ShopAgent, item.itemDetails)
                });
            }

            return toReturn;
        }

        public override void SelectItem(ShopItemUi item, int number = 1)
        {
            agentShopSubSystem.PurchaseItem(ShopAgent, item.Item.itemDetails, 
                currentAdventurerAgent.CurrentAgent.wallet,
                currentAdventurerAgent.CurrentAgent.adventurerInventory.agentInventory);
        }
    }
}
