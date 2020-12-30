using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public class AgentShopSubSystem : LastUpdateClass
    {
        public EnvironmentReset resetScript;
        public List<BaseItemPrices> basePrices;
        private readonly Dictionary<ShopAgent, AgentData> _shopSystems;

        public UsableItem endItem;
        
        public static int SenseCount => AgentData.SenseCount;

        public AgentShopSubSystem()
        {
            _shopSystems = new Dictionary<ShopAgent, AgentData>();
        }

        private AgentData GetShop(ShopAgent shopAgent)
        {
            if (!_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Add(shopAgent, new AgentData(basePrices));
            }

            return _shopSystems[shopAgent];
        }
        
        private List<UsableItem> GetItems(ShopAgent shopAgent)
        {
            var itemList = new List<UsableItem>();
            foreach (var item in shopAgent.agentInventory.Items)
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
                SubmitToShop(agent, shopItem);   
            }
            else
            {
                Debug.Log("Out of range submit");
            }
        }

        public void SubmitToShop(ShopAgent agent, UsableItem item)
        {
            var shop = GetShop(agent);
            shop.SubmitToShop(item);
            agent.agentInventory.RemoveItem(item);
            
            Refresh();
        }

        public int GetPrice(ShopAgent shopAgent, UsableItemDetails item)
        {
            return GetShop(shopAgent).GetPrice(item);
        }

        public List<UsableItem> GetShopItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopItems();
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
            Refresh();
        }

        public int GetNumber(ShopAgent shopAgent, UsableItemDetails item)
        {
            return GetShop(shopAgent).GetNumber(item);
        }

        public void PurchaseItem(ShopAgent shopAgent, UsableItemDetails item, EconomyWallet wallet, AgentInventory inventory)
        {
            var shop = GetShop(shopAgent);
            var success = shop.PurchaseItems(item, wallet, inventory);
            if (success)
            {
                OverviewVariables.SoldItem();
                
                if (item.itemName == endItem.itemDetails.itemName)
                {
                    resetScript.ResetScript();
                }
            }
            Refresh();
        }

        public float[] GetSenses(ShopAgent agent)
        {
            var items = basePrices.Select(b => b.item).ToList();
            return GetShop(agent).GetSenses(items);
        }
    }
}
