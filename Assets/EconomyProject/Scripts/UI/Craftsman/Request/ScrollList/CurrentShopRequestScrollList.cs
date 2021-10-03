using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CurrentShopRequestScrollList : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public RequestShopSystemBehaviour requestShopSystem;
        public GetCurrentShopAgent getCurrentAgent;
        private ShopAgent CraftsmanAgent => getCurrentAgent.CurrentAgent;

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
        
        protected override void Update()
        {
            base.Update();

            var state = requestShopSystem.system.GetState(CraftsmanAgent);
            var resource = requestShopSystem.GetItem(CraftsmanAgent, EShopRequestStates.MakeRequest);
            foreach (var button in buttons)
            {
                button.UpdateData(resource, state);
            }
            Debug.Log(resource);
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
