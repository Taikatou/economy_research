using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using EconomyProject.Scripts.UI.Craftsman.Request.ScrollList;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class AdventurerRequestScrollView : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        protected override List<CraftingResourceRequest> GetItemList()
        {
            var requestTaker = currentAdventurerAgent.CurrentAgent.requestTaker;
            return requestSystem.craftingRequestRecord.GetCurrentRequests(requestTaker);
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
