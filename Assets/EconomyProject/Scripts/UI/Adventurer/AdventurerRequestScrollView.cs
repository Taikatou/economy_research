using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using EconomyProject.Scripts.UI.Craftsman.Request.ScrollList;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class AdventurerRequestScrollView : ShopRequestScrollList<AdventurerCraftingResourceRequest, AdventurerCurrentRequestButton>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        private AdventurerRequestTaker GetCurrentRequestTaker => currentAdventurerAgent?.CurrentAgent?.RequestTaker;

        protected override List<AdventurerCraftingResourceRequest> GetItemList()
        {
            if (GetCurrentRequestTaker == null)
            {
                return null;
            }
            var items = requestSystem.craftingRequestRecord.GetCurrentRequests(GetCurrentRequestTaker);
            var toReturn = new List<AdventurerCraftingResourceRequest>();
            foreach (var item in items)
            {
                var amount = GetCurrentRequestTaker.GetCurrentStock(item.Resource);
                var request = new AdventurerCraftingResourceRequest { Request = item, CurrentNumber = amount };
                toReturn.Add(request);
            }
            return toReturn;
        }

        public override void SelectItem(AdventurerCraftingResourceRequest item, int number = 1)
        {
            // GetCurrentRequestTaker.TakeRequest(item.Request);
        }
    }
}
