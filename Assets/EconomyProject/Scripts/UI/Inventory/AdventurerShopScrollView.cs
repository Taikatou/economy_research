﻿using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
		public GetCurrentShopAgent currentShopAgent;
		
		public AdventurerShopSystemBehaviour adventurerShopSystem;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;

        protected override List<ShopItem> GetItemList()
        {
	        // TODO fix this please
	        var counter = 0;
	        var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(currentShopAgent.CurrentAgent);

	        return items;
        }

        protected override void Update()
        {
	        base.Update();

	        foreach (var button in buttons)
	        {
		        var index = adventurerShopSystem.system.adventurerShopSubSystem.GetCurrentLocation(
			        currentAdventurerAgent.CurrentAgent);
		        button.UpdateData(index, true);
	        }
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {

		}
    }
}
