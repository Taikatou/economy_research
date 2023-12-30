using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors.SystemSensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerSensorComponent : SensorComponent
    {
        public BufferSensorComponent agentShopBufferComponent;
        
        public BaseAdventurerAgent agent;
        public override ISensor[] CreateSensors()
        {
            var behave = FindObjectOfType<AdventurerSystemBehaviour>();

            if (TrainingConfig.SkipShopSetup)
            {
                return new ISensor[] { 
                    new AdventurerBaseSensor(agent),
                    new AgentAdventureSensor(agent, behave.system),
                    new RequestTakerSensor(agent.RequestTaker)
                };
            }
            else
            {
                var request = FindObjectOfType<RequestAdventurerSystemBehaviour>();
                var shop = FindObjectOfType<AdventurerShopSystemBehaviour>();
                var main = FindObjectOfType<MainMenuSystemBehaviour>();

                var sensors = new ISensor [] { 
                    new AdventurerBaseSensor(agent),
                    new AdventurerInventorySensor(agent), 
                    new RequestTakerSensor(agent.RequestTaker),
                    new AgentAdventureSensor(agent, behave.system),
                    new AgentRequestSensor(agent, request.system),
                    new AgentShopSensor(agent, shop.system, agentShopBufferComponent),
                    new AgentMenuSensor(agent, main.system)
                };

                return sensors;
            }
        }
    }
}
