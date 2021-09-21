using System;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public override int ObservationSize => 0;
        public override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(ShopAgent agent)
        {
            return Array.Empty<float>() ;
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            AgentInput.ChangeScreen(agent, (EShopScreen) input);
        }
        
        private void Update()
        {
            RequestDecisions();
        }
    }
}
