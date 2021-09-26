﻿using System;
using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents;

namespace EconomyProject.Scripts.UI.Inventory
{
    public struct ItemUi
    {
        public UsableItemDetails ItemDetails;
        public int Number;
    }
    public struct ShopItem
    {
		public ShopAgent Seller;
        public UsableItem Item;
        public int Number;
        public int Price;
        public int Index;
    }
    public class ShopInventoryScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;

        public GetCurrentShopAgent shopAgent;

        public ShopLocationMap shopLocationMap;
        
        protected override ILastUpdate LastUpdated
		{
			get
			{
				if(shopAgent.CurrentAgent == null)
				{
					return null;
				}
				return shopAgent.CurrentAgent.agentInventory;
			}
		}

        protected override List<ShopItem> GetItemList()
        {
            var itemList = new List<ShopItem>();
            foreach (var item in shopAgent.CurrentAgent.agentInventory.Items)
            {
                var price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent.CurrentAgent, item.Value[0].itemDetails);
                itemList.Add(new ShopItem { Item=item.Value[0], Number=item.Value.Count, Price=price});
            }
            return itemList;
        }

        public override void SelectItem(ShopItem shopItem, int number = 1)
        {
			shopAgent.CurrentAgent.SetAction(EShopAgentChoices.SubmitToShop, null, null, shopItem.Item);
		}

        public void FixedUpdate()
        {
	        var system = shopSubSystem.system.GetState(shopAgent.CurrentAgent);
	        var count = shopLocationMap.GetCurrentLocation(shopAgent.CurrentAgent);
	        foreach (var button in buttons)
	        {
		        button.UpdateData(count, system == ECraftingOptions.SubmitToShop);
	        }
        }
    }
}
