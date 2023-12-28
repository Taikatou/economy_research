using Data;
using EconomyProject.Scripts.MLAgents.Sensors;
using Inventory;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerInventorySensor : BaseEconomySensor
    {
        private readonly BaseAdventurerAgent _agent;

        public AdventurerInventorySensor(BaseAdventurerAgent agent) : base(null)
        {
            _agent = agent;
            var obs = GetData();
            _data = new float [obs.Length];
            MObservationSpec = ObservationSpec.Vector(obs.Length);
        }

        private float[] GetData()
        {
            var item = _agent.AdventurerInventory.EquipedItem;
            var damage = item == null? 0 : item.itemDetails.damage;
            var craft = item == null ? ECraftingChoice.BeginnerSword-1 : item.craftChoice;
            var obs = new ObsData[]
            {
                new SingleObsData
                {
                    data = damage,
                    Name = "name"
                },
                new CategoricalObsData<ECraftingChoice>(craft)
                {
                    Name = "crafting choice",
                },
                new SingleObsData
                {
                    data=_agent.AdventurerInventory.ItemCount / 10,
                    Name="Item Count"
                }
            };
            return ObsData.GetEnumerableData(obs);
        }

        protected override float[] Data => _data;

        private float[] _data;

        public override void Update()
        {
            var obs = GetData();
            _data = new float [obs.Length];
        }

        public override string GetName() => "AdventurerInventorySensor";
    }
}
