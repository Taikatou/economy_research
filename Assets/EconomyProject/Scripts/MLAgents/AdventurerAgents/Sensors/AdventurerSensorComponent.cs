using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerSensorComponent : SensorComponent
    {
        public BufferSensorComponent requestAvailableBufferComponent;
        public BufferSensorComponent agentShopBufferComponent;
        
        public BufferSensorComponent availableRequestsBufferComponent;
        
        public AdventurerAgent agent;
        public override ISensor[] CreateSensors()
        {
            var behave = FindObjectOfType<AdventurerSystemBehaviour>();
            if (behave == null)
            {
                return new ISensor[] { };
            }

            if (TrainingConfig.SkipShopSetup)
            {
                return new ISensor[] { 
                    new AdventurerBaseSensor(agent),
                    new AgentAdventureSensor(agent, behave.system)
                };
            }
            else
            {
                var request = FindObjectOfType<RequestAdventurerSystemBehaviour>();
                var shop = FindObjectOfType<AdventurerShopSystemBehaviour>();
                var main = FindObjectOfType<MainMenuSystemBehaviour>();

                var sensors = new List<ISensor> { 
                    new AdventurerBaseSensor(agent),
                    new AdventurerInventorySensor(agent), 
                    new RequestTakerSensor(requestAvailableBufferComponent, agent.requestTaker),
                    new AgentAdventureSensor(agent, behave.system),
                    new AgentRequestSensor(agent, request.system, availableRequestsBufferComponent),
                    new AgentShopSensor(agent, shop.system, agentShopBufferComponent),
                    new AgentMenuSensor(agent, main.system)
                };

                return sensors.ToArray();
            }
        }
    }
}
