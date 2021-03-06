﻿using System;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        enum MainChoices { Craft = EShopScreen.Craft, Request = EShopScreen.Request }
        public override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(ShopAgent agent)
        {
            return new float [0] ;
        }

        public override InputAction[] GetInputOptions(ShopAgent agent)
        {
            var agentScreen = EconomySystemUtils.GetStateInput<EShopScreen>();
            return agentScreen.ToArray();
        }

        public override void SetChoice(ShopAgent agent, int input)
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
