using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI
{
    public class GetCurrentRequestsLocation : ShopRequestLocationMap
    {
        public RequestSystem requestSystem { get; set; }
        public override int GetLimit(ShopAgent agent)
        {
            var inventory = agent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);
            return items.Count;
        }

        public override ECraftingResources? GetItem(ShopAgent agent)
        {
            ECraftingResources? toReturn = null;
            var inventory = agent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);
            var index = GetCurrentLocation(agent);
            if (index < items.Count)
            {
                toReturn = items[index].Resource;
            }
            return toReturn;
        }
    }
}
