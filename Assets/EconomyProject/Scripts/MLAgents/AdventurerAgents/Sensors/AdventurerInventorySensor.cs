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

            Data = new float [3];
        }

        protected override float[] Data { get; }

        public override void Update()
        {
            var damage = 0;
            var item = _agent.adventurerInventory.EquipedItem;
            if (item != null)
            {
                damage = item.itemDetails.damage;
            }
            Data[0] = damage;
        }

        public override string GetName() => "AdventurerInventorySensor";
    }
}
