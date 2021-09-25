using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class GetCurrentRequestsLocation : LocationSelect<ShopAgent>
    {
        public RequestSystem requestSystem { get; set; }
        protected override int GetLimit(ShopAgent agent)
        {
            var inventory = agent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);
            return items.Count;
        }
    }
}
