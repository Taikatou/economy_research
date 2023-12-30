using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents;

namespace EconomyProject.Scripts.MLAgents
{
    public class EconomyDecisionRequester : DecisionRequester
    {
        public bool adventurer;
        public ShopCraftingSystemBehaviour shopSubSystem;
        public RequestShopSystemBehaviour requestBehaviour;
        public AdventurerRequestTaker requestTaker;
        public void Start()
        {
            shopSubSystem = FindObjectOfType<ShopCraftingSystemBehaviour>();
            requestBehaviour = FindObjectOfType<RequestShopSystemBehaviour>();
        }

        protected override bool AllowDecisions
        {
            get
            {
                if (!adventurer)
                {
                    return true;
                }
                var takenRequests = requestBehaviour.system.requestSystem.craftingRequestRecord.GetCurrentRequests(requestTaker);
                if (takenRequests.Length > 0)
                {
                    return true;
                }
                
                var requests = requestBehaviour.system.requestSystem.GetCraftingRequests();
                if (requests > 0)
                {
                    return true;
                }
                var shopItems = shopSubSystem.system.shopSubSubSystem.GetAllUsableItems();
                return shopItems.Count > 1;
            }
        }
    }
}
