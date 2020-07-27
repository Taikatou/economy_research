using System;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class ShopSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        enum MainChoices { Main = EShopScreen.Main, Craft = EShopScreen.Craft, Request = EShopScreen.Request }
        protected override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override void SetChoice(ShopAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(MainChoices), input))
            {
                var i = (MainChoices) input;
            
                ShopInput.ChangeScreen(agent, (EShopScreen) i);    
            }
        }
        
        private void Update()
        {
            RequestDecisions();
        }
    }
}
