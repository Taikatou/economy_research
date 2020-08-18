using System;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum RequestActions { Quit=CraftingResources.DragonScale+1, SetInput, RemoveRequest, IncreasePrice,  DecreasePrice}
    public class RequestShopSystem : StateEconomySystem<RequestActions, ShopAgent, EShopScreen>
    {
        public RequestSystem requestSystem;
        protected override EShopScreen ActionChoice => EShopScreen.Request;
        protected override RequestActions IsBackState => RequestActions.Quit;
        protected override RequestActions DefaultState => RequestActions.SetInput;
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override void SetChoice(ShopAgent agent, int input)
        {
            if (input >= 0)
            {
                if (Enum.IsDefined(typeof(RequestActions), input))
                {
                    SetInputMode(agent, (RequestActions) input);
                    Debug.Log((RequestActions) input);
                }
                else
                {
                    MakeChoice(agent, input);
                }
            }
        }

        protected override void GoBack(ShopAgent agent)
        {
            AgentInput.ChangeScreen(agent, EShopScreen.Main);
        }

        protected override void MakeChoice(ShopAgent agent, int input)
        {
            var resources = CraftingUtils.CraftingResources.ToList();
            if (input < resources.Count)
            {
                var craftingResources = resources[input];
                MakeChoice(agent, craftingResources);   
            }
        }

        public void MakeChoice(ShopAgent agent, CraftingResources craftingResources)
        {
            switch (GetInputMode(agent))
            {
                case RequestActions.SetInput:
                    Debug.Log("Make Request");
                    requestSystem.MakeRequest(craftingResources, agent.craftingInventory, agent.wallet);
                    break;
                case RequestActions.RemoveRequest:
                    Debug.Log("Remove Request");
                    requestSystem.RemoveRequest(craftingResources, agent.craftingInventory);
                    break;
                case RequestActions.IncreasePrice:
                    Debug.Log("IncreasePrice");
                    requestSystem.ChangePrice(craftingResources, agent.craftingInventory, agent.wallet, 1);   
                    break;
                case RequestActions.DecreasePrice:
                    Debug.Log("DecreasePrice");
                    requestSystem.ChangePrice(craftingResources, agent.craftingInventory, agent.wallet, -1);   
                    break;
            }
        }
        
        private void Update()
        {
            RequestDecisions();
        }
    }
}
