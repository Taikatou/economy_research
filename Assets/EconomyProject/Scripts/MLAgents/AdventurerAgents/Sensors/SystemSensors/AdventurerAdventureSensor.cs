using EconomyProject.Scripts.GameEconomy.Systems;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AdventurerAdventureSensor : AdventurerMovementSensor
    {
        private readonly AdventurerSystem adventurerSystem;
        public override string GetName() => "AdventurerMovementSensor";
        public AdventurerAdventureSensor(AdventurerAgent agent, AdventurerSystem system) : base(agent)
        {
            adventurerSystem = system;
        }

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Adventurer;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => adventurerSystem;
    }
}
