using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Sirenix.Utilities;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public enum EAdventureShopChoices { Up = EAdventurerAgentChoices.Up, Down=EAdventurerAgentChoices.Down, Select=EAdventurerAgentChoices.Select, 
        SetShop = EAdventurerAgentChoices.SetShop, PurchaseItem=EAdventurerAgentChoices.PurchaseItem, Back=EAdventurerAgentChoices.Back }
    
    [Serializable]
    public class AdventurerShopSystem : StateEconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;

        public ShopChooserSubSystem shopChooserSubSystem;

        public override int ObservationSize => 0;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(AdventurerAgent agent)
        {
            return new float[] {};
        }

        public override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            if (Enum.IsDefined(typeof(EAdventureShopChoices), input))
            {
                var agentInput = (EAdventureShopChoices) input;
                switch (agentInput)
                {
                    case EAdventureShopChoices.Select:

                        break;
                    case EAdventureShopChoices.Down:
                        
                        break;
                    case EAdventureShopChoices.Up:

                        break;
                    case EAdventureShopChoices.Back:

                        break;
                    case EAdventureShopChoices.SetShop:
                        shopChooserSubSystem.SetInput(agent, agentInput);
                        break;
                    case EAdventureShopChoices.PurchaseItem:
                        adventurerShopSubSystem.SetInput(agent, agentInput);
                        break;
                }
            }
        }
        
        

        public void Update()
        {
            RequestDecisions();
        }
    }
}
