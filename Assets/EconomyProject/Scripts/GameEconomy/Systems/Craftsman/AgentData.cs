﻿using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using Inventory;
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
    public class AgentData
    {
        private readonly Dictionary<string, int> _defaultPrices;
        private readonly Dictionary<string, int> _stockPrices;
        private readonly Dictionary<string, List<UsableItem>> _shopItems;

        public AgentData(IEnumerable<BaseItemPrices> items)
        {
            _shopItems = new Dictionary<string, List<UsableItem>>();
            _stockPrices = new Dictionary<string, int>();
            
            _defaultPrices = new Dictionary<string, int>();
            foreach(var item in items)
            {
                _defaultPrices.Add(item.item.ToString(), item.price);
            }
        }

        public float[] GetSenses(List<UsableItem> items)
        {
            var output = new float[SenseCount];
            for (var i = 0; i < items.Count; i++)
            {
                var name = items[i].itemDetails.itemName;
                if (_stockPrices.ContainsKey(name))
                {
                    output[i*2] = _stockPrices[name];
                    output[i*2 + 1] = _shopItems[name].Count;
                }
            }

            return output;
        }

        public const int SenseCount = 6 * 2;

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

        public bool PurchaseItems(EconomyWallet shopAgentWallet, UsableItemDetails itemDetails, EconomyWallet adventurerAgentWallet, AgentInventory inventory)
        {
            var price = _stockPrices[itemDetails.itemName];

            int GetStock()
            {
                return _shopItems[itemDetails.itemName].Count;
            }
            
            if (adventurerAgentWallet.Money >= price && GetStock() > 0)
            {
                inventory.AddItem(_shopItems[itemDetails.itemName][0]);
                _shopItems[itemDetails.itemName].RemoveAt(0);
                
                _stockPrices[itemDetails.itemName] = price;

                if (GetStock() <= 0)
                {
                    _stockPrices.Remove(itemDetails.itemName);
                    _shopItems.Remove(itemDetails.itemName);
                }
                
                adventurerAgentWallet.SpendMoney(price);
				shopAgentWallet.EarnMoney(price);

				return true;
            }

            return false;
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

		public int GetStock(UsableItem item)
		{
			return _shopItems[item.itemDetails.itemName].Count;
		}
	}
}
