using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public delegate void OnPurchaseItem(UsableItem item, ShopAgent shop);
    [Serializable]
    public class AgentShopSubSystem : LastUpdateClass
    {
        public EnvironmentReset resetScript;
        public List<BaseItemPrices> basePrices;
        private Dictionary<ShopAgent, AgentData> _shopSystems;

        public OnPurchaseItem OnPurchaseItem;

        public PurchaseItemDataLogger purchaseDataLogger;

        public AgentShopSubSystem()
        {
			ResetShop();
		}

		public void ResetShop()
		{
			_shopSystems = new Dictionary<ShopAgent, AgentData>();
		}

		public AgentData GetShop(ShopAgent shopAgent)
        {
            if (!_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Add(shopAgent, new AgentData(basePrices));
            }

            return _shopSystems[shopAgent];
        }

        public void RemoveShop(ShopAgent shopAgent)
        {
            if (_shopSystems.ContainsKey(shopAgent))
            {
                _shopSystems.Remove(shopAgent);
            }
            Refresh();
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

        public void SubmitToShop(ShopAgent agent, UsableItem item)
        {
			if (agent.agentInventory.ContainsItem(item) == false)
			{
				Debug.Log("Out of range submit. Item : " + item.ToString());
				return;
			}

            var shop = GetShop(agent);
            shop.SubmitToShop(item);
            agent.agentInventory.RemoveItem(item);
            
            Refresh();
        }

        public int GetPrice(ShopAgent shopAgent, UsableItemDetails item)
        {
            return GetShop(shopAgent).GetPrice(item);
        }

        public List<UsableItem> GetShopUsableItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopUsableItems();
        }

        public List<Tuple<UsableItem, ShopAgent>> GetAllUsableItems()
        {
            var output = new List<Tuple<UsableItem, ShopAgent>>();
            foreach (var shop in _shopSystems)
            {
                var items = GetShop(shop.Key).GetShopUsableItems();
                foreach (var item in items)
                {
                    output.Add(new Tuple<UsableItem, ShopAgent>(item, shop.Key));
                }
            }
            return output;
        }
        
        public List<ShopItem> GetShopItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopItems(shopAgent);
        }

        public void SetCurrentPrice(ShopAgent shopAgent, int item, int increment)
		{ 
			var items = GetShopUsableItems(shopAgent);
            if (item < items.Count)
            {
                SetCurrentPrice(shopAgent, items[item].itemDetails, increment);
            }
            Refresh();
        }

        public void SetCurrentPrice(ShopAgent shopAgent, UsableItem item, int increment)
        {
            var items = GetShopUsableItems(shopAgent);
            var itemFound = items.Find(x => x.itemDetails.itemName == item.itemDetails.itemName);
            if(itemFound)
            {
                SetCurrentPrice(shopAgent, itemFound.itemDetails, increment);
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

        public void PurchaseItem(ShopAgent shopAgent, UsableItem item, EconomyWallet wallet, AgentInventory inventory)
        {
            var shop = GetShop(shopAgent);
            var success = shop.PurchaseItems(shopAgent.wallet, item.itemDetails, wallet, inventory, purchaseDataLogger);
            if (success)
            {
                OverviewVariables.SoldItem();
                
                OnPurchaseItem?.Invoke(item, shopAgent);
            }
            Refresh();
        }

        public void UpdateShopSenses(ShopAgent shop, BufferSensorComponent bufferSensorComponent)
        {
            var items = GetShop(shop).GetShopItemsObs(shop);
            foreach (var item in items)
            {
                if (item.Value.HasValue)
                {
                    var outputs = new float[9];
                    outputs[(int)item.Key] = 1;
                    if (item.Value != null)
                    {
                        outputs[7] = item.Value.Value.Price;
                        outputs[7] = item.Value.Value.Item.itemDetails.damage;
                        outputs[8] = item.Value.Value.Item.itemDetails.durability;
                    }
                
                    bufferSensorComponent.AppendObservation(outputs);   
                }
            }
        }

        public void GetItemSenses(BufferSensorComponent bufferSensorComponent, ShopAgent toIgnore)
        {
            foreach (var shop in _shopSystems)
                if (shop.Key != toIgnore)
                {
                    UpdateShopSenses(shop.Key, bufferSensorComponent);
                }
                    
        }

        public static int WeaponList => Enum.GetValues(typeof(ECraftingChoice)).Length;

        public static readonly int SensorCount = WeaponList * 4;
    }
}
