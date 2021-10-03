using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class AdventureRequestLocationSetter : LocationSelect<AdventurerAgent>
    {
        public RequestSystem requestSystem { get; set; }
        protected override int GetLimit(AdventurerAgent agent)
        {
            return requestSystem.GetAllCraftingRequests().Count;
        }

        public CraftingResourceRequest GetRequest(AdventurerAgent agent)
        {
            var items = requestSystem.GetAllCraftingRequests();
            var index = GetCurrentLocation(agent);
            var item = items[index];
            return item;
        }
    }
}
