using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CurrentShopRequestScrollList : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentShopAgent getCurrentAgent;

        // Start is called before the first frame update
        protected override List<CraftingResourceRequest> GetItemList()
        {
            var inventory = getCurrentAgent.CurrentAgent.craftingInventory;
            Debug.Log(inventory + "\t" + requestSystem);
            var items = requestSystem.GetAllCraftingRequests(inventory);
            return items;
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
