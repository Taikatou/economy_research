using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public struct BaseItemPrices
    {
        public UsableItem item;
        public int price;
    }
    public class AgentShop
    {
        private readonly Dictionary<string, int> _defaultPrices;
        private readonly Dictionary<string, int> _stockPrices;
        private readonly Dictionary<string, List<UsableItem>> _shopItems;

        public AgentShop(IEnumerable<BaseItemPrices> items)
        {
            _shopItems = new Dictionary<string, List<UsableItem>>();
            _stockPrices = new Dictionary<string, int>();
            
            _defaultPrices = new Dictionary<string, int>();
            foreach(var item in items)
            {
                _defaultPrices.Add(item.item.ToString(), item.price);
            }
        }

        private void ChangeItem(UsableItem item, int price)
        {
            if (!_stockPrices.ContainsKey(item.ToString()))
            {
                _stockPrices.Add(item.ToString(), price);
                _shopItems.Add(item.ToString(), new List<UsableItem>{item});
            }
            else
            {
                _stockPrices[item.ToString()] = price;
                _shopItems[item.ToString()].Add(item);
            }
        }

        public void SubmitToShop(UsableItem item)
        {
            var price = GetPrice(item.itemDetails);
            
            ChangeItem(item, price);
        }

        public void PurchaseItems(UsableItemDetails itemDetails, EconomyWallet wallet, AgentInventory inventory)
        {
            var price = _stockPrices[itemDetails.itemName];

            int GetStock()
            {
                return _shopItems[itemDetails.itemName].Count;
            }
            
            if (wallet.Money >= price && GetStock() > 0)
            {
                inventory.AddItem(_shopItems[itemDetails.itemName][0]);
                _shopItems[itemDetails.itemName].RemoveAt(0);
                
                _stockPrices[itemDetails.itemName] = price;

                if (GetStock() <= 0)
                {
                    _stockPrices.Remove(itemDetails.itemName);
                    _shopItems.Remove(itemDetails.itemName);
                }
                
                wallet.SpendMoney(price);
            }
            
        }
        public List<UsableItem> GetShopItems()
        {
            var output = new List<UsableItem>();
            foreach(var entry in _shopItems)
            {
                output.Add(entry.Value[0]);
            }

            return output;
        }

        public int GetPrice(UsableItemDetails item)
        {
            if (_defaultPrices.ContainsKey(item.itemName))
            {
                return _defaultPrices[item.itemName];
            }
            return 0;
        }

        public int GetNumber(UsableItemDetails item)
        {
            if (_shopItems.ContainsKey(item.itemName))
            {
                return _shopItems[item.itemName].Count;
            }

            return 0;
        }

        public void SetCurrentPrice(UsableItemDetails item, int increment)
        {
            var price = _defaultPrices[item.itemName];
            _defaultPrices[item.ToString()] = price + increment;
        }
    }
}
