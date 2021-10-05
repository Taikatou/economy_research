using Data;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public interface IShopSense
    {
        ObsData [] GetObservations(ShopAgent agent);
    }
}
