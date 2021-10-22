using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class AdventureRequestLocationSetter : LocationSelect<AdventurerAgent>
    {
        public RequestSystem requestSystem { get; set; }
        public override int GetLimit(AdventurerAgent agent)
        {
            return requestSystem.GetAllCraftingRequests().Count;
        }

        public CraftingResourceRequest GetRequest(AdventurerAgent agent)
        {
            CraftingResourceRequest toReturn = null;
            var items = requestSystem.GetAllCraftingRequests();
            var index = GetCurrentLocation(agent);
            if (items.Count > index && index >= 0)
            {
                toReturn = items[index];
            }

            return toReturn;
        }
    }
}
