using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class AdventurerTakeRequestScrollList : ShopRequestScrollList<CraftingResourceRequest, CraftingCurrentRequestButton>
    {
        public AdventureRequestLocationSetter adventureRequestLocationSetter;
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        protected override List<CraftingResourceRequest> GetItemList()
        {
            var items = requestSystem.GetAllCraftingRequests();
            return items;
        }

        public override void SelectItem(CraftingResourceRequest item, int number = 1)
        {
			// currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.TakeRequest, item);
        }

        protected override void Update()
        {
            base.Update();
            var item = adventureRequestLocationSetter.GetRequest(currentAdventurerAgent.CurrentAgent);
            if (item != null)
            {
                foreach (var button in buttons)
                {
                    button.UpdateData(item.Resource, true);
                }
            }
        }
    }
}
