using EconomyProject.Scripts.GameEconomy.Systems;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentAdventureSensor : AgentMovementSensor<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly AdventurerSystem adventurerSystem;
        public override string GetName() => "AdventurerMovementSensor";
        public AgentAdventureSensor(AdventurerAgent agent, AdventurerSystem system) : base(agent)
        {
            adventurerSystem = system;
        }

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Adventurer;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => adventurerSystem;

        protected override int SensorCount => AdventurerSystem.ObservationSize;
    }
}
