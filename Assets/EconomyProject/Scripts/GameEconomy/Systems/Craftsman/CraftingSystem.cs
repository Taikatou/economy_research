using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    public class CraftingSystem : MonoBehaviour, IShopSense
    {
        public List<CraftingMap> craftingRequirement;
        private Dictionary<ShopAgent, CraftingRequest> _shopRequests;
        private List<ShopAgent> _shopAgents;
        
        public float Progress(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent) ? _shopRequests[agent].Progress : 0.0f;
        }
        
        public void Start()
        {
            _shopRequests = new Dictionary<ShopAgent, CraftingRequest>();
            _shopAgents = new List<ShopAgent>();
        }
        
        public bool HasRequest(ShopAgent agent)
        {
            return _shopRequests.ContainsKey(agent);
        }
        
        private void Update()
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

        public int SenseCount => CraftingRequest.SenseCount;
    }
}
