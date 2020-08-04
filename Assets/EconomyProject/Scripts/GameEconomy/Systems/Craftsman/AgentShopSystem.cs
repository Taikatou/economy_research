using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class AgentShopSystem : MonoBehaviour
    {
        private Dictionary<ShopAgent, AgentShop> _shopSystems;

        private void Start()
        {
            _shopSystems = new Dictionary<ShopAgent, AgentShop>();
        }

        private AgentShop GetShop(ShopAgent shopAgent)
        {
            if (!_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Add(shopAgent, new AgentShop());
            }

            return _shopSystems[shopAgent];
        }

        public void AddItem(ShopAgent agent, InventoryItem item, int price, int stock)
        {
            var shop = GetShop(agent);
            shop.AddItem(item, price, stock);
        }

        public ShopDetails GetShopDetails(ShopAgent agent, InventoryItem item)
        {
            return GetShop(agent).GetStockDetails(item);
        }
    }
}
