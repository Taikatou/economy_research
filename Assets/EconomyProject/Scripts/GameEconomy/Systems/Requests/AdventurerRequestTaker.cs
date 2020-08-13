using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class AdventurerRequestTaker : RequestTaker
    {
        public RequestSystem recordSystem;
        public override List<CraftingResourceRequest> ItemList => recordSystem.GetAllCraftingRequests();
        public override void TakeRequest(CraftingResourceRequest request)
        {
            recordSystem.TakeRequest(this, request);
        }
    }
}
