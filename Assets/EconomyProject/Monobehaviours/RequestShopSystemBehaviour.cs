using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Monobehaviours
{
    public class RequestShopSystemBehaviour : AdvancedLocationSelect<ShopAgent, CraftingResources, EShopRequestStates>
    {
        public GetCurrentRequestsLocation getCurrentRequestsLocation;
        
        public RequestShopSystem system;

        protected override int GetLimit(ShopAgent agent)
        {
            var valuesAsArray = CraftingUtils.GetCraftingResources();
            return valuesAsArray.Count;
        }
        
        public override void Start()
        {
            base.Start();
            system.GetLocation = this;
            system.Start();
            getCurrentRequestsLocation.requestSystem = system.requestSystem;
        }

        public void FixedUpdate()
        {
            system.Update();
        }

        public override CraftingResources GetItem(ShopAgent agent, EShopRequestStates state)
        {
            if (state == EShopRequestStates.MakeRequest)
            {
                var valuesAsList = CraftingUtils.GetCraftingResources();
                var index = GetCurrentLocation(agent);
                return valuesAsList[index];
            }
            else
            {
                var inventory = agent.craftingInventory;
                var items = system.requestSystem.GetAllCraftingRequests(inventory);
                var index = GetCurrentLocation(agent, state);
                return items[index].Resource;
            }
        }

        public override int GetCurrentLocation(ShopAgent agent, EShopRequestStates state)
        {
            return state == EShopRequestStates.MakeRequest
                ? base.GetCurrentLocation(agent)
                : getCurrentRequestsLocation.GetCurrentLocation(agent);
        }
    }
}
