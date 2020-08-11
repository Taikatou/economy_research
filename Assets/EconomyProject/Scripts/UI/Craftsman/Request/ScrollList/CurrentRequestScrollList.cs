using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using UnityEngine;
using ResourceRequest = EconomyProject.Scripts.GameEconomy.Systems.Requests.ResourceRequest;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CurrentRequestScrollList : CraftingScrollList<ResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentAdventurerAgent getCurrentAgent;

        // Start is called before the first frame update
        protected override List<ResourceRequest> GetItemList() => requestSystem.requestSystem.GetAllCraftingRequests();

        public override void SelectItem(ResourceRequest item, int number = 1)
        {
            var requestTaker = getCurrentAgent.CurrentAgent.GetComponent<RequestTaker>();
            Debug.Log(requestTaker == null);
            requestTaker.TakeRequest(item);
        }
    }
}
