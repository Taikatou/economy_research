using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [Serializable]
    public class CraftingSubSystem :  IShopSense
    {
        public List<CraftingMap> craftingRequirement;
        private Dictionary<ShopAgent, CraftingRequest> _shopRequests;
        private List<ShopAgent> _shopAgents;
        
        public static int SenseCount => CraftingRequest.SenseCount;
        
        public float Progress(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent) ? _shopRequests[agent].Progress : 0.0f;
        }
        
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

        public void MakeRequest(ShopAgent shopAgent, int input)
        {
            var foundChoice = craftingRequirement.Single(c => c.choice == (CraftingChoice)input);
            if (foundChoice.resource && !HasRequest(shopAgent))
            {
                var craftingInventory = shopAgent.GetComponent<CraftingInventory>();
                var hasResources = craftingInventory.HasResources(foundChoice.resource);
                if (hasResources)
                {
                    _shopRequests.Add(shopAgent, new CraftingRequest { CraftingRequirements = foundChoice.resource });
                    _shopAgents.Add(shopAgent);
                    Debug.Log("Add request");
                }
            }
        }

        public float[] GetSenses(ShopAgent agent)
        {
            var output = new float [CraftingRequest.SenseCount];
            if (_shopRequests.ContainsKey(agent))
            {
                var senseA = _shopRequests[agent].GetSenses();
                senseA.CopyTo(output, 0);
            }
            return output;
        }

		public Dictionary<ShopAgent, CraftingRequest> GetShopRequests()
		{
			return _shopRequests;
		}
	}
}
