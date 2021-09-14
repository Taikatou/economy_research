using System;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        enum MainChoices { Craft = EShopScreen.Craft, Request = EShopScreen.Request }

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

        public override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            if (Enum.IsDefined(typeof(MainChoices), input))
            {
                var i = (MainChoices) input;
            
                AgentInput.ChangeScreen(agent, (EShopScreen) i);    
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
