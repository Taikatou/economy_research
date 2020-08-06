using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class AgentShop
    {
        private readonly Dictionary<string, int> _defaultPrices;
        private readonly Dictionary<UsableItem, ShopDetails> _stockItems;

        public AgentShop(List<ShopItem> basePrices)
        {
            _stockItems = new Dictionary<UsableItem, ShopDetails>();
            
            _defaultPrices = new Dictionary<string, int>();
            foreach (var item in basePrices)
            {
                _defaultPrices.Add(item.item.itemName, item.shopDetails.price);
            }
        }

        private void ChangeItem(UsableItem item, ShopDetails details)
        {
            if (!_stockItems.ContainsKey(item))
            {
                _stockItems.Add(item, details);
            }
            else
            {
                var stockDetails = _stockItems[item];
                stockDetails.price = details.price;
                stockDetails.stock += details.stock;
            }
        }

        public void SubmitToShop(UsableItem item, ShopDetails details)
        {
            ChangeItem(item, details);

            Debug.Log(_stockItems[item].price + "\t" + _stockItems[item].stock);
        }

        public List<ShopItem> GetShopItems()
        {
            var output = new List<ShopItem>();
            foreach(var entry in _stockItems)
            {
                output.Add(new ShopItem
                {
                    item = entry.Key,
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
