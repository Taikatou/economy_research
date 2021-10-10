using System;
using System.Collections.Generic;
using System.Linq;
using Data;
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

        private static readonly int SensorCount = CraftingAsList.Length + 2;
        public override void Update()
        {
            var screen = (float) _shopAgent.ChosenScreen;
            var walletMoney = _shopAgent.wallet ? _shopAgent.wallet.Money : 0.0f;
            var obsData = new List<ObsData>
            {
                new ObsData { data=walletMoney, name="Shop Wallet" },
                new ObsData { data=screen, name="Chosen Screen"}
            };
            foreach (var resource in CraftingAsList)
            {
                var obs = new ObsData{name = "ResourceCount"};
                if (_shopAgent.craftingInventory.HasResources(resource))
                {
                    var d = _shopAgent.craftingInventory.GetResourceNumber(resource);
                    obs.data = d;
                }
                obsData.Add(obs); 
            }
            

            for (var counter = 0; counter < Data.Length; counter++)
            {
                Data[counter] = obsData[counter].data;
            }
        }

        public ShopBaseSensor(ShopAgent shopAgent)
        {
            _shopAgent = shopAgent;
            Data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }
    }
}
