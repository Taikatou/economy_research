using System;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestShopSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        enum RequestActions { Quit = 6, SetInput=7, RemoveRequest=8, IncreasePrice=9,  DecreasePrice=10}

        private RequestActions _inputMode = RequestActions.SetInput;
        public RequestSystem requestSystem;
        protected override EShopScreen ActionChoice => EShopScreen.Request;
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
                    _inputMode = (RequestActions) input;
                    Debug.Log(_inputMode);
                }
                else
                {
                    var craftingResources = CraftingUtils.CraftingResources.ToList();
                    MakeChoice(agent, craftingResources[input]);
                }
            }
        }

        public void MakeChoice(ShopAgent agent, CraftingResources craftingResources)
        {
            switch (_inputMode)
            {
                case RequestActions.Quit:
                    ShopInput.ChangeScreen(agent, EShopScreen.Main);
                    break;
                case RequestActions.SetInput:
                    Debug.Log("Make Request");
                    requestSystem.MakeRequest(craftingResources, agent.CraftingInventory);
                    break;
                case RequestActions.RemoveRequest:
                    requestSystem.RemoveRequest(craftingResources, agent.CraftingInventory);
                    break;
                case RequestActions.IncreasePrice:
                    Debug.Log("IncreasePrice");
                    requestSystem.IncreasePrice(craftingResources, agent.CraftingInventory);   
                    break;
                case RequestActions.DecreasePrice:
                    Debug.Log("DecreasePrice");
                    requestSystem.DecreasePrice(craftingResources, agent.CraftingInventory);   
                    break;
            }
        }
        
        private void Update()
        {
            RequestDecisions();
        }
    }
}
