using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman
{
    [System.Serializable]
    public struct CraftingMap
    {
        public CraftingChoice choice;
        public CraftingRequirements resource;
    }
    
    public struct CraftingRequest
    {
        public CraftingRequirements CraftingRequirements;
        public float CraftingTime;
        
        public float Progress => CraftingTime / CraftingRequirements.timeToCreation;

        public void Update(float time)
        {
            CraftingTime += time;
        }

        public bool Complete()
        {
            return CraftingTime >= CraftingRequirements.timeToCreation;
        }
    }

    public enum CraftingChoice { Abstain, BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, UltimateSwordOfPower}

    public class CraftingSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        public List<CraftingMap> craftingRequirement;
        public Dictionary<ShopAgent, CraftingRequest> shopRequests;

        protected override EShopScreen ActionChoice => EShopScreen.Craft;

        public override float Progress(ShopAgent agent)
        {
            return shopRequests.ContainsKey(agent) ? shopRequests[agent].Progress : 0.0f;
        }

        public override bool CanMove(ShopAgent agent)
        {
            return !shopRequests.ContainsKey(agent);
        }

        public void Start()
        {
            shopRequests = new Dictionary<ShopAgent, CraftingRequest>();
        }

        public void SetCraftingItem(CraftingChoice choice, ShopAgent shopAgent)
        {
            var foundChoice = craftingRequirement.Single(c => c.choice == choice);
            if (foundChoice.resource && CanMove(shopAgent))
            {
                var craftingInventory = shopAgent.GetComponent<CraftingInventory>();
                var hasResources = craftingInventory.HasResources(foundChoice.resource);
                if (hasResources)
                {
                    shopRequests.Add(shopAgent, new CraftingRequest
                    {
                        CraftingRequirements = foundChoice.resource,
                        CraftingTime = 0
                    });
                }
            }
        }
        
        private void Update()
        {
            var toRemove = new List<ShopAgent>();
            foreach (var entry in shopRequests)
            {
                entry.Value.Update(Time.deltaTime);
                if (entry.Value.Complete())
                {
                    var generatedItem = InventoryItem.GenerateItem(entry.Value.CraftingRequirements.resultingItem);
                    var agentInventory = entry.Key.GetComponent<AgentInventory>();
                    agentInventory.AddItem(generatedItem);

                    toRemove.Add(entry.Key);
                }
            }
            foreach(var item in toRemove)
            {
                shopRequests.Remove(item);
            }
        }
    }
}
