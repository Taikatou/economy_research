using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerSensorComponent : SensorComponent
    {
        public BufferSensorComponent requestTakenBufferComponent;
        public BufferSensorComponent requestAvailableBufferComponent;

        public BufferSensorComponent agentShopBufferComponent;
        
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
                    new AgentAdventureSensor(agent, behave.system, null)
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
                    new RequestTakerSensor(requestAvailableBufferComponent, requestTakenBufferComponent, 
                        agent.requestTaker),
                    new AgentAdventureSensor(agent, behave.system, null),
                    new AgentRequestSensor(agent, request.system),
                    new AgentShopSensor(agent, shop.system, agentShopBufferComponent),
                    new AgentMenuSensor(agent, main.system)
                };

                return sensors.ToArray();
            }
        }
    }
}
