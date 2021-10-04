using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AdventurerShopSensor : AdventurerMovementSensor
    {
        private readonly AdventurerShopSystem shopSystem;

        public override string GetName() => "AdventurerShopSensor";

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Shop;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => shopSystem;

        protected override int SensorCount => AdventurerShopSystem.ObservationSize;
        
        public AdventurerShopSensor(AdventurerAgent agent, AdventurerShopSystem system) : base(agent)
        {
            shopSystem = system;
        }
    }
}
