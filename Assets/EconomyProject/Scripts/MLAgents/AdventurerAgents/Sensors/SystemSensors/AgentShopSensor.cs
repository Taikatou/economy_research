using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentShopSensor : AgentMovementSensor<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly AdventurerShopSystem shopSystem;

        public override string GetName() => "AdventurerShopSensor";

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Shop;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => shopSystem;

        protected override int SensorCount => AdventurerShopSystem.ObservationSize;

        public AgentShopSensor(AdventurerAgent agent, AdventurerShopSystem system) : base(agent)
        {
            shopSystem = system;
        }
    }
}
