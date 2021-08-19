using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftsmanRequestTaker : RequestTaker
    {
        public ShopAgent craftsMan;
        public override IEnumerable<CraftingResourceRequest> GetItemList() => new List<CraftingResourceRequest>();

        public override void TakeRequest(CraftingResourceRequest request)
        {
            // craftsMan.requestSystem.RemoveRequest(request, craftsMan.CraftingInventory);
        }

        public override void CompleteRequest(int reward)
        {
            throw new System.NotImplementedException();
        }
    }
}
