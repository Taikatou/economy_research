using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerSensorComponent : SensorComponent
    {
        public int requestLimit = 3;
        public AdventurerAgent agent;
        public override ISensor[] CreateSensors()
        {
            var behave = FindObjectOfType<AdventurerSystemBehaviour>();

            if (ConfigSystem.skipShopSetup)
            {
                return new ISensor[] { 
                    new AdventurerBaseSensor(agent),
                    new AdventurerInventorySensor(agent), 
                    new AgentAdventureSensor(agent, behave.system)
                };
            }
            else
            {
                var request = FindObjectOfType<RequestAdventurerSystemBehaviour>();
                var shop = FindObjectOfType<AdventurerShopSystemBehaviour>();
                return new ISensor[] { 
                    new AdventurerBaseSensor(agent),
                    new AdventurerInventorySensor(agent), 
                    new RequestTakerSensor(agent.requestTaker, requestLimit),
                    new AgentAdventureSensor(agent, behave.system),
                    new AgentRequestSensor(agent, request.system),
                    new AgentShopSensor(agent, shop.system)
                };   
            }
        }
    }
}
