using System;
using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<ECraftingChoice, List<UsableItem>>  GetShopUsableItems(ShopAgent shopAgent)
        {
            return GetShop(shopAgent).GetShopUsableItems();
        }
        
        public List<Tuple<UsableItem, ShopAgent>> GetAllUsableItems(bool ignore=true, ECraftingChoice choice=ECraftingChoice.BeginnerSword, bool outputImmediate=false)
        {
            var output = new List<Tuple<UsableItem, ShopAgent>>();
            foreach (var shop in _shopSystems)
            {
                var items = GetShop(shop.Key).GetShopUsableItems();
                foreach (var itemPair in items)
                {
                    if (ignore || choice == itemPair.Key)
                    {
                        foreach (var item in itemPair.Value)
                        {
                            output.Add(new Tuple<UsableItem, ShopAgent>(item, shop.Key));
                            if (outputImmediate)
                                return output;
                        }
                    }
                }
            }
            return output;
        }
        
        public Tuple<UsableItem, ShopAgent> GetLowestPriceOfItem(ECraftingChoice choice)
        {
            int price = Int32.MaxValue;
            Tuple<UsableItem, ShopAgent> output = null;
            foreach (var shop in _shopSystems)
            {
                var items = GetShop(shop.Key).GetShopUsableItems();
                foreach (var itemPairs in items)
                {
                    if (itemPairs.Key == choice)
                    {
                        foreach (var item in itemPairs.Value)
                        {
                            var itemPrice= GetPrice(shop.Key, item.itemDetails);
                            if (price > itemPrice)
                            {
                                output = new Tuple<UsableItem, ShopAgent>(item, shop.Key);
                            }
                        }
                        
                    }
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
            throw new NotImplementedException();
        }

        public void SetCurrentPrice(ShopAgent shopAgent, ECraftingChoice craftingChoice, int increment)
        {
            var items = GetShopUsableItems(shopAgent);
            if (items.TryGetValue(craftingChoice, out var item))
            {
                var itemFound = item.Find(x => x.craftChoice == craftingChoice);
                if(itemFound)
                {
                    SetCurrentPrice(shopAgent, itemFound.itemDetails, increment);
                }
                Refresh();   
            }
        }

        public void SetCurrentPrice(ShopAgent shopAgent, UsableItemDetails item, int increment)
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

        // get prices of all items in shop
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
                        outputs[8] = 0;
                    }
                
                    bufferSensorComponent.AppendObservation(outputs);   
                }
            }

            foreach (var item in shop.agentInventory.Items)
            {
                if (item.Key.Contains("Sword"))
                {
                    var outputs = new float[9];
                    outputs[(int)item.Value[0].craftChoice] = 1;
                    var price = GetPrice(shop, item.Value[0].itemDetails);
                    if (item.Value != null)
                    {
                        outputs[7] = price;
                        outputs[7] = item.Value.Count;
                        outputs[8] = 1;
                    }
                
                    bufferSensorComponent.AppendObservation(outputs);   
                }
            }
        }

        readonly ECraftingChoice[] _craftingChoices = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>().ToArray();
        
        public void GetItemSenses(BufferSensorComponent bufferSensorComponent, ShopAgent toIgnore)
        {
            foreach (var choice in _craftingChoices)
            {
                var lowestPrice = GetLowestPriceOfItem(choice);
                if (lowestPrice != null)
                {
                    var outputs = new float[9];
                    outputs[(int)choice] = 1;
                    if (lowestPrice.Item1 != null)
                    {
                        outputs[7] = GetPrice(lowestPrice.Item2, lowestPrice.Item1.itemDetails);
                        outputs[7] = lowestPrice.Item1.itemDetails.damage;
                        outputs[8] = lowestPrice.Item1.itemDetails.durability;
                    }
                    bufferSensorComponent.AppendObservation(outputs);
                }
            }
        }

        public static int WeaponList => Enum.GetValues(typeof(ECraftingChoice)).Length;

        public static readonly int SensorCount = WeaponList * 4;
    }
}
