using System;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        enum MainChoices { Craft = EShopScreen.Craft, Request = EShopScreen.Request }
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
            else if(input >= 0)
            {
                Debug.Log(input);
            }
        }
        
        private void Update()
        {
            RequestDecisions();
        }
    }
}
