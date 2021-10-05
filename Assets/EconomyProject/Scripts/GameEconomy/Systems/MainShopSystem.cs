using System;
using Data;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(ShopAgent agent)
        {
            return Array.Empty<ObsData>() ;
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            var s = (EShopScreen) input;
            if (Enum.IsDefined(typeof(EShopScreen), s))
            {
                AgentInput.ChangeScreen(agent, s);
            }
        }

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
        {
            var inputChoices = new []
            {
                EShopAgentChoices.Craft,
                EShopAgentChoices.Resources
            };
            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
            return outputs;
        }
    }
}
