using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : MonoBehaviour, IShopSubSystem
    {
        public AgentShopSubSystem agentShopSubSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        private Dictionary<AdventurerAgent, int> _currentLocation;

        public void Start()
        {
            _currentLocation = new Dictionary<AdventurerAgent, int>();
        }

        public void SetInput(AdventurerAgent agent, AdventureShopInput choice)
        {
            if (!_currentLocation.ContainsKey(agent))
            {
                _currentLocation.Add(agent, 0);
            }

            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = agentShopSubSystem.GetShopItems(shopAgent);
            switch (choice)
            {
                case AdventureShopInput.Up:
                    MovePosition(agent, shopItems, 1);
                    break;
                case AdventureShopInput.Down:
                    MovePosition(agent, shopItems, -1);
                    break;
                case AdventureShopInput.Select:
                    var shopDetails = shopItems[_currentLocation[agent]].itemDetails;
                    agentShopSubSystem.PurchaseItem(shopAgent, shopDetails, agent.wallet, agent.inventory);
                    break;
            }
        }

        private void MovePosition(AdventurerAgent agent, List<UsableItem> shopItems, int movement)
        {
            var newPosition = _currentLocation[agent] + movement;
            if (newPosition >= 0 && newPosition < shopItems.Count)
            {
                _currentLocation[agent] = newPosition;
            }
        }
    }
}
