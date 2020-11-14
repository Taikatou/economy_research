using System;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class OverViewMenu : MonoBehaviour
    {
        public Text currentRequestsText, requestScreenTextAdv, requestScreenTextCrft, shopText;

        public RequestSystem requestSystem;

        public AdventurerInput adventurerInput;

        public ShopInput shopInput;

        public AgentShopSubSystem agentShopSubSystem;

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
            var currentRequests = requestSystem.GetAllCraftingRequests().Count;
            currentRequestsText.text = "Requests: " + currentRequests;

            var requestScreenAdv = adventurerInput.GetCount(EAdventurerScreen.Request);
            requestScreenTextAdv.text = "Agents in request Screen: " + requestScreenAdv;

            var requestScreenCraft = shopInput.GetCount(EShopScreen.Craft);
            requestScreenTextCrft.text = "Craft in request Screen: " + requestScreenCraft;

            var counter = 0;
            var shopAgent = FindObjectsOfType<ShopAgent>();
            foreach (var agent in shopAgent)
            {
                var shopItems = agentShopSubSystem.GetShopItems(agent);
                counter += shopItems.Count;
            }

            shopText.text = "Shop item count: " + counter;
        }
    }
}
