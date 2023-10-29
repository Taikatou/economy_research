using System;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Inventory;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    public class ShopHeuristic : MonoBehaviour
    {
        private System.Random generator;
        public void Start()
        {
            generator = new System.Random();
        }

        public void Heuristic(RequestSystem requestSystem, ShopCraftingSystem shopCraftingSystem, ShopAgent agent)
        {
            var items = requestSystem.GetAllCraftingRequests(agent.craftingInventory);
            var requestMade = false;
            var epsilon = generator.Next(20);
            if (items.Count < 2 || epsilon == 2)
            {
                var requestChoice = ECraftingResources.Wood;
                var currentRequests = requestSystem.GetAllCraftingRequestsObservations(agent.craftingInventory);
                foreach (var request in items)
                {
                    if(!currentRequests.ContainsKey(requestChoice))
                    {
                        break;
                    }
                    requestChoice++;
                }
                var success = requestSystem.MakeRequest(requestChoice, agent.craftingInventory, agent.wallet);
                requestMade = success;
            }

            var craftingMade = false;
            if(!requestMade)
            {
                if (agent.agentInventory.Items.Count < 2)
                {
                    var choices = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>();
                    foreach (var c in choices.Reverse())
                    {
                        if (shopCraftingSystem.craftingSubSubSystem.CanCraft(agent, c))
                        {
                            shopCraftingSystem.craftingSubSubSystem.MakeCraftRequest(agent, c);
                            craftingMade = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
