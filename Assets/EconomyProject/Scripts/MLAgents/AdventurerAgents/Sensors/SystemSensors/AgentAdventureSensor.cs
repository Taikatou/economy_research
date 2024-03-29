using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentAdventureSensor : AgentMovementSensor<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly AdventurerSystem _adventurerSystem;
        public override string GetName() => "AdventurerMovementSensor";
        public AgentAdventureSensor(AdventurerAgent agent, AdventurerSystem system) : base(agent, null)
        {
            _adventurerSystem = system;
        }

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Adventurer;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => _adventurerSystem;

        protected override int SensorCount => AdventurerSystem.ObservationSize;
    }
}
