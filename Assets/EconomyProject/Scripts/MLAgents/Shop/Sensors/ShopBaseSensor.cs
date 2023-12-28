using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopBaseSensor : BaseEconomySensor
    {
        private static readonly ECraftingResources [] CraftingAsList = Enum.GetValues(typeof(ECraftingResources)).Cast<ECraftingResources>().ToArray();
        private readonly ShopAgent _shopAgent;
        protected override float[] Data { get; }
        public override string GetName() => "ShopBaseSensor";

        private readonly ShopMainLocationSelect _locationSelect;

        private float[] UpdateData()
        {
            var walletMoney = _shopAgent.wallet ? _shopAgent.wallet.Money : 0.0f;
            var obsData = new List<ObsData>
            {
                new SingleObsData { data=walletMoney, Name="Shop Wallet" },
            };
            
            foreach (var resource in CraftingAsList)
            {
                var obs = new SingleObsData
                {
                    Name="ResourceCount"
                };
                if (_shopAgent.craftingInventory.HasResources(resource))
                {
                    var d = _shopAgent.craftingInventory.GetResourceNumber(resource);
                    obs.data = d;
                }
                obsData.Add(obs); 
            }

            return ObsData.GetEnumerableData(obsData);
        }

        public override void Update()
        {
            var observations = UpdateData();
            for (var counter = 0; counter < Data.Length; counter++)
            {
                Data[counter] = observations[counter];
            }
        }

        public ShopBaseSensor(ShopAgent shopAgent, ShopMainLocationSelect locationSelect) : base(null)
        {
            _locationSelect = locationSelect;
            _shopAgent = shopAgent;
            var data = UpdateData();
            Data = new float [data.Length];
            MObservationSpec = ObservationSpec.Vector(data.Length);
        }
    }
}
