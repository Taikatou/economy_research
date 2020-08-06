using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class InventoryScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public AgentShopSystem shopSystem;

        public GetCurrentShopAgent shopAgent;
        protected override LastUpdate LastUpdated => shopAgent.CurrentAgent.AgentInventory;

        protected override List<ShopItem> GetItemList()
        {
            var itemList = new List<ShopItem>();
            Debug.Log(shopAgent.CurrentAgent.AgentInventory.Items.Keys.Count);
            foreach (var item in shopAgent.CurrentAgent.AgentInventory.Items)
            {
                var price = shopSystem.GetCurrentPrice(shopAgent.CurrentAgent, item.Value.Item);
                var shopDetails = new ShopDetails { price = price, stock = item.Value.Number };
                itemList.Add(new ShopItem { item = item.Value.Item, shopDetails = shopDetails });
            }
            return itemList;
        }

        public override void SelectItem(ShopItem shopItem, int number = 1)
        {
            shopSystem.SubmitToShop(shopAgent.CurrentAgent, shopItem.item, shopItem.shopDetails.stock);
        }
    }
}
