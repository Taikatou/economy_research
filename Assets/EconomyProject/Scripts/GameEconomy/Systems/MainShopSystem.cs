using System;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public ShopMainLocationSelect mainLocationSelect;
        public override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            return new ObsData[]
            {
                new CategoricalObsData<EShopScreen>(mainLocationSelect.GetMenu(agent))
            };
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            switch (input)
            {
                
            }
        }

        public override EShopAgentChoices[] GetEnabledInputs(ShopAgent agent)
        {
            EShopAgentChoices[] inputChoices = new EShopAgentChoices []{};
            return inputChoices;
        }
    }
}
