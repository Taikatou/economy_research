using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Sensors;
using Inventory;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopListItemsSensor : BaseEconomySensor
    {
        protected override float[] Data { get; }

        private readonly ShopAgent _shopAgent;

        private readonly ShopCraftingSystem _shopSubSystem;

        private readonly BufferSensorComponent _bufferSensorComponent;
    
        public ShopListItemsSensor(ShopAgent shopAgent, ShopCraftingSystem shopSubSystem, BufferSensorComponent bufferSensorComponent) : base(new []{bufferSensorComponent})
        {
            _shopSubSystem = shopSubSystem;
            _shopAgent = shopAgent;
            MObservationSpec = ObservationSpec.Vector(0);
            Data = Array.Empty<float>();
            _bufferSensorComponent = bufferSensorComponent;
        }
    
        public override void Update()
        {
            var items = _shopSubSystem.shopSubSubSystem.GetShopUsableItems(_shopAgent);
            
            foreach (var itemPairs in items)
            {
                foreach (var item in itemPairs.Value)
                {
                    var obs = new List<float>
                    {
                        (float)_shopSubSystem.shopSubSubSystem.GetPrice(_shopAgent, item.itemDetails) / TrainingConfig.MaxPrice,
                        (float)_shopSubSystem.shopSubSubSystem.GetNumber(_shopAgent, item.itemDetails) / 10
                    };
                    var itemType = new float[Enum.GetValues(typeof(ECraftingChoice)).Length];
                    itemType[(int)item.craftChoice] = 1;
                    obs.AddRange(itemType);
                    _bufferSensorComponent.AppendObservation(obs.ToArray());
                }
            }
        }

        public override string GetName() => "ShopListItemsSensor";

    }
}
