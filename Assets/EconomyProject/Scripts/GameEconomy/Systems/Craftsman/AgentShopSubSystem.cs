using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public class AgentShopSubSystem : LastUpdateClass
    {
        public EnvironmentReset resetScript;
        public List<BaseItemPrices> basePrices;
        private Dictionary<ShopAgent, AgentData> _shopSystems;

		public UsableItem endItem;

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

        public void PurchaseItem(ShopAgent shopAgent, UsableItemDetails item, EconomyWallet wallet, AgentInventory inventory)
        {
            var shop = GetShop(shopAgent);
            var success = shop.PurchaseItems(shopAgent.wallet, item, wallet, inventory);
            if (success)
            {
                OverviewVariables.SoldItem();
                
                if (item.itemName == endItem.itemDetails.itemName)
                {
                    //resetScript.ResetScript();
                }
            }
            Refresh();
        }

        public ObsData[] GetItemSenses(ShopAgent agent)
        {
            var items = GetShop(agent).GetShopItems(agent);
            var itemsObs = GetWeaponObservations(items);

            return itemsObs;
        }

        public static int WeaponList => Enum.GetValues(typeof(ECraftingChoice)).Length;

        public static readonly int SensorCount = WeaponList * 4;
        
        public ObsData[] GetWeaponObservations(List<ShopItem> items)
        {
            var outputs = new ObsData[WeaponList * 4];
            for(var i = 0; i < WeaponList; i++)
            {
                var index = i * 4;
                outputs[index] = new ObsData { name = "craftingChoice" };
                outputs[index + 1] = new ObsData { name = "itemPrice" };
                outputs[index = 2] = new ObsData { name = "itemDamage" };
                outputs[index + 3] = new ObsData { name = "itemDurability" };
            }
            var valuesAsArray = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>().ToArray();
            var counter = 0;
            foreach (var craft in valuesAsArray)
            {
                var f = items.Any(i => craft == i.Item.craftChoice);
                if (f)
                {
                    var i = items.First(i => craft == i.Item.craftChoice);
                    outputs[counter] = new ObsData
                    {
                        data=(float) i.Item.craftChoice,
                        name="craftingChoice"
                    };
                    outputs[counter + 1] = new ObsData
                    {
                        data=i.Price,
                        name="itemPrice"
                    };
                    outputs[counter + 2] = new ObsData
                    {
                        data=i.Item.itemDetails.damage,
                        name="itemDamage"
                    };
                    outputs[counter + 3] = new ObsData
                    {
                        data=i.Item.itemDetails.durability,
                        name="itemDurability"
                    };
                }
                counter += 4;
            }

            while (counter < outputs.Length)
            {
                
            }

            return outputs;
        }
    }
}
