using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public struct ShopItem
    {
        public InventoryItem Item;
        public ShopDetails ShopDetails;
    }
    public class InventoryScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public AgentShopSystem shopSystem;

        public GetCurrentShopAgent shopAgent;
        private AgentInventory AgentInventory => shopAgent.CurrentAgent.AgentInventory;
        public override LastUpdate LastUpdated => AgentInventory;
        public override List<ShopItem> ItemList
        {
            get
            {
                var itemList = new List<ShopItem>();
                foreach (var item in AgentInventory.Items)
                {
                    var shopDetails = shopSystem.GetShopDetails(shopAgent.CurrentAgent, item);
                    itemList.Add(new ShopItem {Item = item, ShopDetails = shopDetails});
                }
                return itemList;
            }
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
