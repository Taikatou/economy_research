using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class AgentShopSystem : LastUpdate
    {
        public List<BaseItemPrices> basePrices;
        private Dictionary<ShopAgent, AgentShop> _shopSystems;

        private void Start()
        {
            _shopSystems = new Dictionary<ShopAgent, AgentShop>();
        }

        private AgentShop GetShop(ShopAgent shopAgent)
        {
            var items = new List<UsableItem>();
            var prices = new List<int>();
            foreach (var item in basePrices)
            {
                items.Add(item.item);
                prices.Add(item.price);
            }
            
            if (!_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Add(shopAgent, new AgentShop(basePrices));
            }

            return _shopSystems[shopAgent];
        }
        
        private List<UsableItem> GetItems(ShopAgent shopAgent)
        {
            var itemList = new List<UsableItem>();
            foreach (var item in shopAgent.AgentInventory.Items)
            {
                itemList.Add(item.Value[0]);
            }
            return itemList;
        }

        public void SubmitToShop(ShopAgent agent, int item)
        {
            var items = GetItems(agent);
            if (item < items.Count)
            {
                var shopItem = items[item];
                SubmitToShop(agent, shopItem, 1);   
            }
            else
            {
                Debug.Log("Out of range submit");
            }
        }

        public void SubmitToShop(ShopAgent agent, UsableItem item, int stock)
        {
            var shop = GetShop(agent);
            var price = GetCurrentPrice(agent, item.itemDetails);

            shop.SubmitToShop(item, price);
            agent.AgentInventory.RemoveItem(item, stock);
            
            Refresh();
        }

        public int GetCurrentPrice(ShopAgent shopAgent, UsableItemDetails item)
        {
            return GetShop(shopAgent).GetCurrentPrice(item);
        }

        public List<UsableItem> GetShopItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopItems();
        }

        public int GetItemPrice(ShopAgent shopAgent, UsableItemDetails itemDetails)
        {
            return GetShop(shopAgent).GetPrice(itemDetails);
        }

        public void SetCurrentPrice(ShopAgent shopAgent, int item, int increment)
        {
            var items = GetItems(shopAgent);
            if (item < items.Count)
            {
                SetCurrentPrice(shopAgent, items[item].itemDetails, increment);
            }
            Refresh();
        }

        private void SetCurrentPrice(ShopAgent shopAgent, UsableItemDetails item, int increment)
        {
            GetShop(shopAgent).SetCurrentPrice(item, increment);
        }

        public void PurchaseItem(ShopAgent shopAgent, UsableItemDetails item, EconomyWallet wallet, AgentInventory inventory)
        {
            GetShop(shopAgent).PurchaseItems(item, wallet, inventory);
        }
    }
}
