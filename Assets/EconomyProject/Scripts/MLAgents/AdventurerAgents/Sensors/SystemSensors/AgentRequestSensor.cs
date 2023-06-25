using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentRequestSensor : AgentMovementSensor<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly RequestAdventurerSystem _requestAdventurerSystem;

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Request;

        protected override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => _requestAdventurerSystem;

        protected override int SensorCount => RequestAdventurerSystem.ObservationSize;

        public override string GetName() => "RequestSensor";

        public AgentRequestSensor(AdventurerAgent agent, RequestAdventurerSystem requestAdventurerSystem) : base(agent, null)
        {
            _requestAdventurerSystem = requestAdventurerSystem;
        }
    }
}
