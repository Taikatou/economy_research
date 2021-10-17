using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Monobehaviours
{
    public class RequestShopSystemBehaviour : AdvancedLocationSelect<ShopAgent, ECraftingResources?, EShopRequestStates>
    {
        public GetCurrentRequestsLocation getCurrentRequestsLocation;
        
        public RequestShopSystem system;

        protected override int GetLimit(ShopAgent agent)
        {
            var valuesAsArray = CraftingUtils.GetCraftingResources();
            return valuesAsArray.Count;
        }

        public void Start()
        {
            system.GetLocation = this;
            getCurrentRequestsLocation.requestSystem = system.requestSystem;
        }

        public override void Setup()
        {
            base.Setup();
            system.Setup();
        }

        public override ECraftingResources? GetItem(ShopAgent agent, EShopRequestStates state)
        {
            ECraftingResources? toReturn = null;
            if (state == EShopRequestStates.MakeRequest)
            {
                var valuesAsList = CraftingUtils.GetCraftingResources();
                var index = GetCurrentLocation(agent);
                if (index < valuesAsList.Count)
                {
                    toReturn = valuesAsList[index];
                }
            }
            else
            {
                var inventory = agent.craftingInventory;
                var items = system.requestSystem.GetAllCraftingRequests(inventory);
                var index = GetCurrentLocation(agent, state);
                if (index < items.Count)
                {
                    toReturn = items[index].Resource;
                }
            }

            return toReturn;
        }

        public override int GetCurrentLocation(ShopAgent agent, EShopRequestStates state)
        {
            return state == EShopRequestStates.MakeRequest
                ? base.GetCurrentLocation(agent)
                : getCurrentRequestsLocation.GetCurrentLocation(agent);
        }
    }
}
