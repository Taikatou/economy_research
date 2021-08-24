using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerMovementSensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;

        public override string GetName() => "MovementSystem";
        
        protected override float[] Data { get; }

        public AdventurerMovementSensor(AdventurerAgent agent)
        {
            _agent = agent;
            
            Data = new float [2];
            MObservationSpec = ObservationSpec.Vector(2);
        }
        
        public override void Update()
        {
            _agent.adventurerInput.GetObservations(_agent);
        }
    }
}
