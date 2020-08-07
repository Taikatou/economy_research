using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class AgentShop
    {
        private readonly Dictionary<string, int> _defaultPrices;
        private readonly Dictionary<string, ShopDetails> _stockItems;
        private readonly Dictionary<string, UsableItem> _shopItems;

        public AgentShop(List<ShopItem> basePrices)
        {
            _shopItems = new Dictionary<string, UsableItem>();
            _stockItems = new Dictionary<string, ShopDetails>();
            
            _defaultPrices = new Dictionary<string, int>();
            foreach (var item in basePrices)
            {
                _defaultPrices.Add(item.item.itemName, item.shopDetails.price);
            }
        }

        private void ChangeItem(UsableItem item, ShopDetails details)
        {
            if (!_stockItems.ContainsKey(item.itemName))
            {
                _stockItems.Add(item.itemName, details);
                _shopItems.Add(item.itemName, item);
            }
            else
            {
                var stockDetails = _stockItems[item.itemName];
                stockDetails.price = details.price;
                stockDetails.stock += details.stock;
            }
        }

        public void SubmitToShop(UsableItem item, ShopDetails details)
        {
            ChangeItem(item, details);

            Debug.Log(_stockItems[item.itemName].price + "\t" + _stockItems[item.itemName].stock);
        }

        public void PurchaseItems(UsableItem item, EconomyWallet wallet, AgentInventory inventory)
        {
            var price = _stockItems[item.itemName].price;
            if (wallet.Money >= price)
            {
                var numItem = _stockItems[item.itemName].stock - 1;
                _stockItems[item.itemName] = new ShopDetails{price=price, stock=numItem};
                
                inventory.AddNewItem(_shopItems[item.itemName]);
                
                if (_stockItems[item.itemName].stock <= 0)
                {
                    _stockItems.Remove(item.itemName);
                    _shopItems.Remove(item.itemName);
                }
                
                wallet.SpendMoney(price);
            }
        }

        public List<ShopItem> GetShopItems()
        {
            var output = new List<ShopItem>();
            foreach(var entry in _stockItems)
            {
                output.Add(new ShopItem
                {
                    item = _shopItems[entry.Key],
                    shopDetails = entry.Value
                });
            }

            return output;
        }

        public int GetCurrentPrice(UsableItem item)
        {
            if (_defaultPrices.ContainsKey(item.itemName))
            {
                return _defaultPrices[item.itemName];
            }
            return 0;
        }

        public void SetCurrentPrice(UsableItem item, int increment)
        {
            var price = _defaultPrices[item.itemName];
            _defaultPrices[item.itemName] = price + increment;
        }
    }
}
