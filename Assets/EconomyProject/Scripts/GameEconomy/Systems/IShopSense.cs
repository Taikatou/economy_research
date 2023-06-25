using Data;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public interface IShopSense
    {
        ObsData [] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent);
    }
}
