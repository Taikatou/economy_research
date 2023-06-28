using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Sensors;
using Inventory;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopInventorySensor : BaseEconomySensor
    {
        protected override float[] Data { get; }

        private readonly ShopAgent _shopAgent;

        private readonly ShopCraftingSystem _shopSubSystem;

        private readonly BufferSensorComponent _bufferSensorComponent;

        public override void Update()
        {
            foreach (var item in _shopAgent.agentInventory.Items)
            {
                var price = _shopSubSystem.shopSubSubSystem.GetPrice(_shopAgent, item.Value[0].itemDetails);
                var obs = new List<float> {(float)
                    item.Value.Count / 10, (float)price/TrainingConfig.MaxPrice };
                var itemType = new float[Enum.GetValues(typeof(ECraftingChoice)).Length];
                itemType[(int)item.Value[0].craftChoice] = 1;
                obs.AddRange(itemType);
                _bufferSensorComponent.AppendObservation(obs.ToArray());
            }
        }

        public override string GetName() => "ShopSensor";

        public ShopInventorySensor(ShopAgent shopAgent, ShopCraftingSystem shopSubSystem, BufferSensorComponent bufferSensorComponent) : base(null)
        {
            _shopSubSystem = shopSubSystem;
            _shopAgent = shopAgent;
            MObservationSpec = ObservationSpec.Vector(0);
            Data = Array.Empty<float>();
            _bufferSensorComponent = bufferSensorComponent;
        }
    }
}
