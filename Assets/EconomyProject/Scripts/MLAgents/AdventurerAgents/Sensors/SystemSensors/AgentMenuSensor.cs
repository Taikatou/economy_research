using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentMenuSensor : AgentMovementSensor<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly MainMenuSystem _adventurerSystem;
        
        public AgentMenuSensor(BaseAdventurerAgent agent, MainMenuSystem system) : base(agent, null)
        {
            _adventurerSystem = system;
        }

        public override string GetName() => "AgentMenuSystem";

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Main;

        protected override EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem =>
            _adventurerSystem;

        protected override int SensorCount => 0;
    }
}
