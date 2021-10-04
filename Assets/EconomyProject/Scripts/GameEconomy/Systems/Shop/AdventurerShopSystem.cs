using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{

    public enum ESelectionState { PurchaseItem, SelectShop }

    public delegate void SetChoice(AdventurerAgent agent);
    
    [Serializable]
    public class AdventurerShopSystem : StateEconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;

        public ShopChooserSubSystem shopChooserSubSystem;

        public Dictionary<AdventurerAgent, ESelectionState> currentStates;

        public static int ObservationSize => 1 + ShopChooserSubSystem.SensorCount + AdventurerShopSubSystem.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public AdventurerShopSystem()
        {
            currentStates = new Dictionary<AdventurerAgent, ESelectionState>();
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(AdventurerAgent agent)
        {
            var choice = GetChoice(agent);
            var obs = new List<float>{ (float) choice };

            var shopSelectData = new float[ShopChooserSubSystem.SensorCount];
            if (choice == ESelectionState.SelectShop)
            {
                shopSelectData = shopChooserSubSystem.GetObservations(agent);
            }
            obs.AddRange(shopSelectData);

            var purchaseItem = new float[AdventurerShopSubSystem.SensorCount];
            if (choice == ESelectionState.PurchaseItem)
            {
                purchaseItem = adventurerShopSubSystem.GetObservations(agent);
            }
            obs.AddRange(purchaseItem);
            
            return obs.ToArray();
        }

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            switch (input)
            {
                case EAdventurerAgentChoices.Down:
                    GetCurrentSubsystem(agent).MovePosition(agent, -1);
                    break;
                
                case EAdventurerAgentChoices.Up:
                    GetCurrentSubsystem(agent).MovePosition(agent, 1);
                    break;
                
                case EAdventurerAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);

                    break;
                case EAdventurerAgentChoices.SetShop:
                    SetChoice(agent, ESelectionState.SelectShop);
                    break;
                
                case EAdventurerAgentChoices.PurchaseItem:
                    SetChoice(agent, ESelectionState.PurchaseItem, adventurerShopSubSystem.PurchaseItem);
                    break;
            }
        }

        private IMoveMenu<AdventurerAgent> GetCurrentSubsystem(AdventurerAgent agent)
        {
            return GetChoice(agent) == ESelectionState.PurchaseItem ? (IMoveMenu<AdventurerAgent>) adventurerShopSubSystem : 
                                                                      (IMoveMenu<AdventurerAgent>) shopChooserSubSystem;
        }

        public ESelectionState GetChoice(AdventurerAgent agent, ESelectionState defaultChoice=ESelectionState.SelectShop)
        {
            if (!currentStates.ContainsKey(agent))
            {
                currentStates.Add(agent, defaultChoice);
            }

            return currentStates[agent];
        }

        private void SetChoice(AdventurerAgent agent, ESelectionState state, SetChoice on=null)
        {
            if (currentStates.ContainsKey(agent))
            {
                var secondOption = currentStates[agent] == state;
                if (secondOption && on != null)
                {
                    on.Invoke(agent);
                }
                currentStates[agent] = state;
            }
            else
            {
                currentStates.Add(agent, state);
            }
        }

        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Back,
                EAdventurerAgentChoices.SetShop,
                EAdventurerAgentChoices.PurchaseItem
            };
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
