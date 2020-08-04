using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class AgentShop
    {
        private readonly Dictionary<InventoryItem, ShopDetails> _stockItems;

        public AgentShop()
        {
            _stockItems = new Dictionary<InventoryItem, ShopDetails>();
        }

        private void CheckKey(InventoryItem item)
        {
            if (!_stockItems.ContainsKey(item))
            {
                _stockItems.Add(item, new ShopDetails());
            }
        }

        public void AddItem(InventoryItem item, int price, int stock)
        {
            CheckKey(item);
            _stockItems[item].AddStock(stock);
            _stockItems[item].SetPrice(price);
        }

        public ShopDetails GetStockDetails(InventoryItem item)
        {
            CheckKey(item);
            return new ShopDetails();
        }
    }
}
