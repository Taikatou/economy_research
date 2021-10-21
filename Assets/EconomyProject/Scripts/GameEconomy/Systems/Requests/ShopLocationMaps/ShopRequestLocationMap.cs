using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps
{
    public abstract class ShopRequestLocationMap : LocationSelect<ShopAgent>
    {
        public abstract ECraftingResources? GetItem(ShopAgent agent);
    }
}
