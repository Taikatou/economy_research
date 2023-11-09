using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentShopSensor : AgentMovementSensor<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly AdventurerShopSystem shopSystem;

        public override string GetName() => "AdventurerShopSensor";

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Shop;

        protected override EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => shopSystem;

        protected override int SensorCount => AdventurerShopSystem.ObservationSize;

        public AgentShopSensor(BaseAdventurerAgent agent, AdventurerShopSystem system, BufferSensorComponent buffer) : base(agent, new []{buffer})
        {
            shopSystem = system;
        }
    }
}
