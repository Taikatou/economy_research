using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerInventorySensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;

        public AdventurerInventorySensor(AdventurerAgent agent)
        {
            _agent = agent;
            MObservationSpec = ObservationSpec.Vector(1);

            Data = new float [1];
        }

        protected override float[] Data { get; }

        public override void Update()
        {
            var damage = _agent.adventurerInventory.EquipedItem.itemDetails.damage;
            Data[0] = damage;
        }

        public override string GetName() => "AdventurerInventorySensor";
    }
}
