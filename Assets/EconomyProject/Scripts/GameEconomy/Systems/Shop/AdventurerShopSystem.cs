using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

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

        public static int SensorSize => 2;

        public static int ObservationSize => SensorSize + ShopChooserSubSystem.SensorCount + AdventurerShopSubSystem.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;

        public int GetScrollLocation(AdventurerAgent agent)
        {
            var index = 0;
            var state = GetChoice(agent);
            switch (state)
            {
                case ESelectionState.PurchaseItem:
                    index = adventurerShopSubSystem.GetCurrentLocation(agent);
                    break;
                case ESelectionState.SelectShop:
                    index = shopChooserSubSystem.GetCurrentLocation(agent);
                    break;
            }
            return index;
        }
        public AdventurerShopSystem()
        {
            currentStates = new Dictionary<AdventurerAgent, ESelectionState>();
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent)
        {
            var choice = GetChoice(agent);
            var obs = new List<ObsData>
            {
                new ObsData
                {
                    data = (float) choice,
                    name = "Choice"
                },
                new ObsData
                {
                    data=GetScrollLocation(agent),
                    name="scrollLocation",
                }
            };

            var shopSelectData =  shopChooserSubSystem.GetObservations(agent);
            obs.AddRange(shopSelectData);

            var purchaseItem =  adventurerShopSubSystem.GetObservations(agent);
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
            var inputChoices = new List<EAdventurerAgentChoices>
            {
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Back
            };

            var stateAction = GetChoice(agent) == ESelectionState.PurchaseItem
                ? EAdventurerAgentChoices.SetShop
                : EAdventurerAgentChoices.PurchaseItem;
            inputChoices.Add(stateAction);
            
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
