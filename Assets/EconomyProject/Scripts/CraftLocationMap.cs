using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.Inventory;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class CraftLocationMap : LocationSelect<ShopAgent>
    {
        public ShopCraftingSystem craftingSystem { get; set; }
        // Start is called before the first frame update
        public override int GetLimit(ShopAgent agent)
        {
            var items = GetData(agent);
            return items.Count;
        }

        private List<ShopItem> GetData(ShopAgent agent)
        {
            var itemList = new List<ShopItem>();
            foreach (var item in agent.agentInventory.Items)
            {
                var price = craftingSystem.shopSubSubSystem.GetPrice(agent, item.Value[0].itemDetails);
                itemList.Add(new ShopItem { Item=item.Value[0], Number=item.Value.Count, Price=price});
            }

            return itemList;
        }
    
        public ShopItem? GetShopItemChoice(ShopAgent agent)
        {
            ShopItem? toReturn = null;
            var items = GetData(agent);
            var index = GetCurrentLocation(agent);
            if (index < items.Count && index >= 0)
            {
                toReturn = items[index];
            }
            return toReturn;
        }
    }
}
