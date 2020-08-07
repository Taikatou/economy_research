using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public struct ShopItem
    {
        public UsableItem item;
        public ShopDetails shopDetails;
    }
    public class AgentShopSystem : LastUpdate
    {
        public List<ShopItem> basePrices;
        private Dictionary<ShopAgent, AgentShop> _shopSystems;

        private void Start()
        {
            _shopSystems = new Dictionary<ShopAgent, AgentShop>();
        }

        private AgentShop GetShop(ShopAgent shopAgent)
        {
            if (!_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Add(shopAgent, new AgentShop(basePrices));
            }

            return _shopSystems[shopAgent];
        }

        public void SubmitToShop(ShopAgent agent, int item)
        {
            var items = GetItems(agent);
            if (item < items.Count)
            {
                var shopItem = items[item];
                SubmitToShop(agent, shopItem.item, shopItem.shopDetails.stock);   
            }
            else
            {
                Debug.Log("Out of range submit");
            }
        }

        public void SubmitToShop(ShopAgent agent, UsableItem item, int stock)
        {
            var shop = GetShop(agent);
            var price = GetCurrentPrice(agent, item);
            var shopItem = new ShopDetails{price=price, stock=stock};
            
            shop.SubmitToShop(item, shopItem);
            agent.AgentInventory.RemoveItem(item, stock);
            
            Refresh();
        }

        public int GetCurrentPrice(ShopAgent shopAgent, UsableItem item)
        {
            return GetShop(shopAgent).GetCurrentPrice(item);
        }

        public List<ShopItem> GetShopItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopItems();
        }

        private List<ShopItem> GetItems(ShopAgent shopAgent)
        {
            var itemList = new List<ShopItem>();
            foreach (var item in shopAgent.AgentInventory.Items)
            {
                var price = GetCurrentPrice(shopAgent, item.Value.Item);
                var shopDetails = new ShopDetails {price = price, stock=item.Value.Number};
                itemList.Add(new ShopItem {item = item.Value.Item, shopDetails = shopDetails});
            }
            return itemList;
        }

        public void SetCurrentPrice(ShopAgent shopAgent, int item, int increment)
        {
            var items = GetItems(shopAgent);
            if (item < items.Count)
            {
                SetCurrentPrice(shopAgent, items[item].item, increment);
            }
            Refresh();
        }

        private void SetCurrentPrice(ShopAgent shopAgent, UsableItem item, int increment)
        {
            GetShop(shopAgent).SetCurrentPrice(item, increment);
        }

        public void PurchaseItem(ShopAgent shopAgent, UsableItem item, EconomyWallet wallet, AgentInventory inventory)
        {
            GetShop(shopAgent).PurchaseItems(item, wallet, inventory);
        }
    }
}
