using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps
{
    public class ShopMakeCraftingRequestLocationMap : ShopRequestLocationMap
    {
        public override int GetLimit(ShopAgent agent)
        {
            var valuesAsArray = CraftingUtils.GetCraftingResources();
            return valuesAsArray.Count;
        }

        public override ECraftingResources? GetItem(ShopAgent agent)
        {
            var valuesAsList = CraftingUtils.GetCraftingResources();
            var index = GetCurrentLocation(agent);
            return valuesAsList[index];
        }
    }
}
