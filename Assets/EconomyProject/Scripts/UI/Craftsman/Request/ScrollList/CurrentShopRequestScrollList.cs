using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CurrentShopRequestScrollList : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentShopAgent getCurrentAgent;

        protected override List<CraftingResourceRequest> GetItemList()
        {
			if(getCurrentAgent.CurrentAgent == null)
			{
				return new List<CraftingResourceRequest>();
			}

            var inventory = getCurrentAgent.CurrentAgent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);
            return items;
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
