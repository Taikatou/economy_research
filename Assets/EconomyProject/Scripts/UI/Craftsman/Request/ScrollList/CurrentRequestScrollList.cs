using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CurrentRequestScrollList : CraftingScrollList<ResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentShopAgent getCurrentAgent;

        private RequestTaker RequestTaker => getCurrentAgent.CurrentAgent.GetComponent<RequestTaker>();

        // Start is called before the first frame update
        protected override List<ResourceRequest> GetItemList() => requestSystem.requestSystem.GetAllCraftingRequests();

        public override void SelectItem(ResourceRequest item, int number = 1)
        {
            RequestTaker.TakeRequest(item);
        }
    }
}
