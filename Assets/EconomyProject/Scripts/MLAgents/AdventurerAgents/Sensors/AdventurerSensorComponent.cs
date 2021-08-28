using EconomyProject.Scripts.MLAgents.AdventurerAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerSensorComponent : SensorComponent
    {
        public int requestLimit = 3;
        public AdventurerAgent agent;
        public override ISensor[] CreateSensors()
        {
            return new ISensor[] { 
                new AdventurerBaseSensor(agent),
                new AdventurerInventorySensor(agent), 
                new RequestTakerSensor(agent.requestTaker, requestLimit),
                new AdventurerMovementSensor(agent)
            };
        }
    }
}
