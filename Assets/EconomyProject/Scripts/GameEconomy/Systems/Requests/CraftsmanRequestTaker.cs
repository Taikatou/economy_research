using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftsmanRequestTaker : RequestTaker
    {
        public ShopAgent craftsMan;
        public override List<CraftingResourceRequest> ItemList { get; }

        public override void TakeRequest(CraftingResourceRequest request)
        {
            // craftsMan.requestSystem.RemoveRequest(request, craftsMan.CraftingInventory);
        }
    }
}
