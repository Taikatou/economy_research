﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public class CraftingSubSystem : IShopSense
    {
        public List<CraftingMap> craftingRequirement;
        private Dictionary<ShopAgent, CraftingRequest> _shopRequests;
        private List<ShopAgent> _shopAgents;

        public static int SenseCount => TrainingConfig.IgnoreCraftTime? 0 : CraftingRequest.SenseCount;
        
        public float Progress(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent) ? _shopRequests[agent].Progress : 0.0f;
        }
        
        public AgentShopSubSystem shopSubSubSystem;
        
        public CraftingSubSystem()
        {
            _shopRequests = new Dictionary<ShopAgent, CraftingRequest>();
            _shopAgents = new List<ShopAgent>();
        }

		public void ResetCraftingSubSystem()
		{
			_shopRequests = new Dictionary<ShopAgent, CraftingRequest>();
			_shopAgents = new List<ShopAgent>();
		}
        
        public bool HasRequest(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent);
        }

        public bool IsCrafting(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent);
        }
        
        public void Update()
        {
            var toRemove = new List<ShopAgent>();
            foreach (var agent in _shopAgents)
            {
                _shopRequests[agent].CraftingTime += Time.deltaTime;
                if (_shopRequests[agent].Complete)
                {
                    Debug.Log("Complete");
                    var generatedItem = UsableItem.GenerateItem(_shopRequests[agent].CraftingRequirements.resultingItem);
                    agent.agentInventory.AddItem(generatedItem);

                    if (TrainingConfig.SubmitDirectToShop)
                    {
                        Task.Run(delegate
                        {
                            SubmitToShop(agent, generatedItem);
                            return Task.CompletedTask;
                        });
                    }
                    OverviewVariables.CraftItem();
                      
                    toRemove.Add(agent);
                }
            }
            
            foreach(var item in toRemove)
            {
                _shopRequests.Remove(item);
                _shopAgents.Remove(item);
            }
        }

        private async Task SubmitToShop(ShopAgent agent, UsableItem generatedItem)
        {
            await Task.Delay(1000);
            shopSubSubSystem.SubmitToShop(agent, generatedItem);
        }

        public void MakeCraftRequest(ShopAgent shopAgent, ECraftingChoice input)
        {
            var foundChoice = GetMap(input);
            if (CanCraft(shopAgent, input))
            {
                _shopRequests.Add(shopAgent, new CraftingRequest { CraftingRequirements = foundChoice.resource });
                _shopAgents.Add(shopAgent);
                Debug.Log("Add request");
            }
        }

        public CraftingMap GetMap(ECraftingChoice input) => craftingRequirement.Single(c => c.choice == input);

        public bool CanCraft(ShopAgent shopAgent, ECraftingChoice input)
        {
            if (TrainingConfig.IgnoreResources)
                return true;
            
            var toReturn = false;
            var foundChoice = GetMap(input);
            var hasRequest = HasRequest(shopAgent);
            if (foundChoice.resource && !hasRequest)
            {
                var craftingInventory = shopAgent.GetComponent<CraftingInventory>();
                var hasResources = craftingInventory.HasResources(foundChoice.resource);
                toReturn = hasResources;
            }

            return toReturn;
        }

        public ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            if (TrainingConfig.IgnoreCraftTime)
            {
                return new ObsData[] { };
            }
            else
            {
                var hasProgress = new SingleObsData { data = IsCrafting(agent) ? 1.0f : 0.0f };
                var output = IsCrafting(agent)
                    ? _shopRequests[agent].GetCraftingProgressionObservation()
                    : new SingleObsData { data = 0, Name = "CraftingProgress" };
                return new[] { hasProgress, output };   
            }
        }

		public Dictionary<ShopAgent, CraftingRequest> GetShopRequests()
		{
			return _shopRequests;
		}
	}
}
