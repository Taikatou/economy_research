using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class AdventureRequestLocationSetter : LocationSelect<BaseAdventurerAgent>
    {
        public RequestSystem requestSystem { get; set; }
        public override int GetLimit(BaseAdventurerAgent agent)
        {
            return requestSystem.GetAllCraftingRequests().Count;
        }

        public CraftingResourceRequest GetRequest(BaseAdventurerAgent agent)
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
