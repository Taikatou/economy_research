using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum RequestActions { Quit=CraftingResources.DragonScale+1, SetInput, RemoveRequest, IncreasePrice,  DecreasePrice}
    
    [Serializable]
    public class RequestShopSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public RequestSystem requestSystem;
        public override int ObservationSize => CraftingResourceRequest.SensorCount + 1;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(ShopAgent agent)
        {
            var outputSenses = new float[CraftingResourceRequest.SensorCount + 1];
            outputSenses[0] = 0; //(float) GetInputMode(agent);
            var requestSense = requestSystem.GetObservations(agent);
            requestSense.CopyTo(outputSenses, 1);
            return outputSenses;
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
           /* if (input >= 0)
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
            }*/
        }

        public void MakeChoice(ShopAgent agent, CraftingResources craftingResources)
        {
            /*switch (GetInputMode(agent))
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
            }*/
        }
        
        public void Update()
        {
            RequestDecisions();
        }
    }
}
