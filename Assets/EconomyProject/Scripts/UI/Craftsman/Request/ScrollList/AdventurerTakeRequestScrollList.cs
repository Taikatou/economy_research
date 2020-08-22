using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class AdventurerTakeRequestScrollList : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        protected override List<CraftingResourceRequest> GetItemList()
        {
            var items = requestSystem.GetAllCraftingRequests();
            return items;
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
            var requestTaker = currentAdventurerAgent.CurrentAgent.GetComponent<RequestTaker>();
            Debug.Log(requestTaker == null);
            requestTaker.TakeRequest(item);
        }
    }
}
