using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.Inventory
{
	public class ShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentShopAgent shopAgent;
        
        private ShopAgent Agent => shopAgent.CurrentAgent;
        
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;
        // Update is called once per frame
        protected override List<ShopItem> GetItemList()
        {
            var toReturn = new List<ShopItem>();
            var items = shopSubSystem.system.shopSubSubSystem.GetShopUsableItems(Agent);
            var counter = 0;
            foreach (var item in items)
            {
                toReturn.Add(new ShopItem
                {
                    Item = item,
                    Price = shopSubSystem.system.shopSubSubSystem.GetPrice(Agent, item.itemDetails),
                    Number = shopSubSystem.system.shopSubSubSystem.GetNumber(Agent, item.itemDetails),
                    Index = counter
                });
                counter++;
            }

            return toReturn;
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {
			Debug.Log("Remove from shop : " + item.Item.itemDetails.itemName);
            throw new System.NotImplementedException();
        }

		public void IncreasePrice(ShopItem item)
		{
			shopAgent.CurrentAgent.SetAction(EShopAgentChoices.IncreasePrice, null, null, item.Item);
		}

		public void DecreasePrice(ShopItem item)
		{
			shopAgent.CurrentAgent.SetAction(EShopAgentChoices.DecreasePrice, null, null, item.Item);
		}

		public void FixedUpdate()
		{
			var system = shopSubSystem.system.GetState(Agent);
			var index = shopSubSystem.shopLocationMap.GetCurrentLocation(Agent);

			foreach (var button in buttons)
			{
				button.UpdateData(index, system == ECraftingOptions.EditShop);
			}
		}
    }
}
