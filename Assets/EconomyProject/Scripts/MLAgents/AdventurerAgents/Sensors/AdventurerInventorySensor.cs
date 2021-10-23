using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerInventorySensor : BaseEconomySensor
    {
        private static int SensorCount => 3;
        private readonly AdventurerAgent _agent;

        public AdventurerInventorySensor(AdventurerAgent agent)
        {
            _agent = agent;
            MObservationSpec = ObservationSpec.Vector(SensorCount);

            Data = new float [SensorCount];
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
            Data[1] = WeaponUtils.NameHashTable[item.itemDetails.itemName];
            Data[2] = _agent.adventurerInventory.ItemCount;
        }

        public override string GetName() => "AdventurerInventorySensor";
    }
}
