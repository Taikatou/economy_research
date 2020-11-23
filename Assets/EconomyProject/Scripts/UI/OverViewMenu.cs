using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class OverViewMenu : MonoBehaviour
    {
        public Text currentRequestsText, requestScreenTextAdv, requestScreenTextCrft, shopText, shopItems,
        agentsInBattleText, advInAdventureText, itemSoldText, battlesWonText, shopMoneyText, openWoodRequestText,
        openMetalRequestText, craftingRequestText, shopCountText, openGemRequestText, openDragonRequestText;

        public RequestSystem requestSystem;

        public AdventurerInput adventurerInput;

        public ShopInput shopInput;

        public AgentShopSubSystem agentShopSubSystem;

        public AdventurerSystem adventurerSystem;

        private void Start()
        {
            var showPlayMenus = Math.Abs(Time.timeScale - 1) < 0.01f;
            if (showPlayMenus)
            {
                // gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            Dictionary<CraftingResources, int> CreateCounter(IEnumerable<CraftingResources> elements)
            {
                return elements.ToDictionary(key => key, key => 0);
            }
            var keys = new[] {
                CraftingResources.Nothing, CraftingResources.Wood, CraftingResources.Metal, 
                CraftingResources.Gem, CraftingResources.DragonScale
            };
            
            var craftingCount = CreateCounter(keys);
            var craftingPrice = CreateCounter(keys);

            int resourceCount = 0, itemCount = 0, shopMoney = 0, shopCount = 0;
            var shopAgent = FindObjectsOfType<ShopAgent>();
            
            foreach (var agent in shopAgent)
            {
                var shopItems = agentShopSubSystem.GetShopItems(agent);
                itemCount += shopItems.Count;
                resourceCount += agent.craftingInventory.GetResourceNumber();

                shopMoney += agent.GetComponent<EconomyWallet>().Money;
                foreach (var key in keys)
                {
                    craftingCount[key] += agent.craftingInventory.GetResourceNumber(key);
                }

                shopCount += agent.agentInventory.Items.Count;
            }
            
            foreach (var request in requestSystem.GetAllCraftingRequests())
            {
                craftingPrice[request.Resource] += request.Reward;
            }
            
            var currentRequests = requestSystem.GetAllCraftingRequests().Count;
            currentRequestsText.text = "Open Requests: " + currentRequests;

            var requestScreenAdv = adventurerInput.GetCount(EAdventurerScreen.Request);
            requestScreenTextAdv.text = "Agents in request Screen: " + requestScreenAdv;

            var requestScreenCraft = shopInput.GetCount(EShopScreen.Craft);
            requestScreenTextCrft.text = "Craft in request Screen: " + requestScreenCraft;

            shopText.text = "Items for sale: " + itemCount;

            shopItems.text = "Resource Count: " + resourceCount;

            agentsInBattleText.text = "In Battle: " + adventurerSystem.GetBattleCount();
            
            advInAdventureText.text = "In Adventure: " + adventurerInput.GetCount(EAdventurerScreen.Adventurer);

            itemSoldText.text = "Items sold: " + OverviewVariables.ShopItemsSold;

            battlesWonText.text = "Battles Won: " + OverviewVariables.BattlesWon;

            shopMoneyText.text = "Money: " + shopMoney;

            openWoodRequestText.text = "Wood Requests: " + craftingCount[CraftingResources.Wood] + ": " + craftingPrice[CraftingResources.Wood];
            
            openMetalRequestText.text = "Metal Requests: " + craftingCount[CraftingResources.Metal] + ": " + craftingPrice[CraftingResources.Metal];

            craftingRequestText.text = "Crafted Items: " + OverviewVariables.CraftingCount;

            shopCountText.text = "Shop Item Count: " + shopCount;
        }
    }
}
