using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class AdventurerRequestTaker : RequestTaker
    {
        public RequestSystem recordSystem;
        public override List<ResourceRequest> ItemList => recordSystem.GetAllCraftingRequests();
        public override void TakeRequest(ResourceRequest request)
        {
            recordSystem.TakeRequest(this, request);
        }
    }
}
