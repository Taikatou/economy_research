using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Sensors
{
    public class CraftingSensor : BaseEconomySensor
    {
        private readonly ShopAgent _shopAgent;
        private readonly CraftingSubSystem _craftingSubSystem;
        private float[] _data;
            
        public CraftingSensor(ShopAgent agent, CraftingSubSystem craftingSubSystem) : base(null)
        {
            _shopAgent = agent;
            _data = new float[1];
            _craftingSubSystem = craftingSubSystem;
            MObservationSpec = ObservationSpec.Vector(1);
        }

        protected override float[] Data => _data;

        public override void Update()
        {
            var outputData = new List<float>();
            var obs = _craftingSubSystem.GetObservations(_shopAgent, BufferSensorComponent);
            foreach (var ob in obs)
            {
                outputData.AddRange(ob.GetData);
            }

            _data = outputData.ToArray();
        }

        public override string GetName()
        {
            return "CraftingSensor";
        }
    }
}
