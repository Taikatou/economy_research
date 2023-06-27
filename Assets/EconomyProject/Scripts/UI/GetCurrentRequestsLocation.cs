using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI
{
    public class GetCurrentRequestsLocation : ShopRequestLocationMap
    {
        public RequestSystem RequestSystem { get; set; }
        public override int GetLimit(ShopAgent agent)
        {
            var inventory = agent.craftingInventory;
            var items = RequestSystem.GetAllCraftingRequests(inventory);
            return items.Count;
        }

        public override ECraftingResources? GetItem(ShopAgent agent)
        {
            ECraftingResources? toReturn = null;
            var inventory = agent.craftingInventory;
            var items = RequestSystem.GetAllCraftingRequests(inventory);
            var index = GetCurrentLocation(agent);
            if (index < items.Count && index >= 0)
            {
                toReturn = items[index].Resource;
            }
            return toReturn;
        }
    }
}
