using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentMenuSensor : AgentMovementSensor<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly MainMenuSystem _adventurerSystem;
        
        public AgentMenuSensor(AdventurerAgent agent, MainMenuSystem system) : base(agent, null)
        {
            _adventurerSystem = system;
        }

        public override string GetName() => "AgentMenuSystem";

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Main;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem =>
            _adventurerSystem;

        protected override int SensorCount => AdventurerSystemLocationSelect.SensorCount;
    }
}
