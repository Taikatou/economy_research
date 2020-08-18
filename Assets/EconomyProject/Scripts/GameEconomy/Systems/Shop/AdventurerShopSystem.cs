using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public enum AdventureShopInput {Up = AdventureShopChoices.Back + 1, Down, Select}
    public enum AdventureShopChoices { SetShop, PurchaseItem, Back }
    public class AdventurerShopSystem : StateEconomySystem<AdventureShopChoices, AdventurerAgent, AgentScreen>
    {
        public AgentShopSubSystem agentShopSubSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        protected override AgentScreen ActionChoice => AgentScreen.Shop;
        protected override AdventureShopChoices IsBackState => AdventureShopChoices.Back;
        protected override AdventureShopChoices DefaultState => AdventureShopChoices.SetShop;

        public ShopAgent GetCurrentShop(AdventurerAgent agent)
        {
            return shopChooserSubSystem.GetCurrentShop(agent);
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(AdventurerAgent agent)
        {
            //var items = shopSubSystem.GetShopItems(_currentShop);
            return new[] {0.0f};
        }
        protected override void MakeChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AdventureShopInput), input))
            {
                var agentInput = (AdventureShopInput) input;
                switch (GetInputMode(agent))
                {
                    case AdventureShopChoices.SetShop:
                        shopChooserSubSystem.SetInput(agent, agentInput);
                        break;
                    case AdventureShopChoices.PurchaseItem:
                        
                        break;
                }
            }
        }

        protected override void GoBack(AdventurerAgent agent)
        {
            AgentInput.ChangeScreen(agent, AgentScreen.Main);
        }

        public void Update()
        {
            RequestDecisions();
        }
    }
}
