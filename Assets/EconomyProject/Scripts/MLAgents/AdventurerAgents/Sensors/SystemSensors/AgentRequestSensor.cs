using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors
{
    public class AgentRequestSensor : AgentMovementSensor<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private readonly RequestAdventurerSystem _requestAdventurerSystem;

        protected override EAdventurerScreen ValidScreen => EAdventurerScreen.Request;

        protected override EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
            => _requestAdventurerSystem;

        protected override int SensorCount => 0;

        public override string GetName() => "RequestSensor";

        public AgentRequestSensor(BaseAdventurerAgent agent, RequestAdventurerSystem requestAdventurerSystem, BufferSensorComponent availableRequests) : base(agent, new []{availableRequests})
        {
            _requestAdventurerSystem = requestAdventurerSystem;
        }
    }
}
