using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;
using UnityEngine.UIElements;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : MonoBehaviour, IShopSubSystem, IAdventureSense
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        private Dictionary<AdventurerAgent, int> _currentLocation;

        public void Start()
        {
            _currentLocation = new Dictionary<AdventurerAgent, int>();
        }

        public void SetInput(AdventurerAgent agent, EAdventureShopChoices choice)
        {
            if (!_currentLocation.ContainsKey(agent))
            {
                _currentLocation.Add(agent, 0);
            }

            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
            switch (choice)
            {
                case EAdventureShopChoices.Up:
                    MovePosition(agent, shopItems, 1);
                    break;
                case EAdventureShopChoices.Down:
                    MovePosition(agent, shopItems, -1);
                    break;
                case EAdventureShopChoices.Select:
                    if (_currentLocation[agent] < shopItems.Count)
                    {
                        var shopDetails = shopItems[_currentLocation[agent]].itemDetails;
                        shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(shopAgent, shopDetails, agent.wallet, agent.inventory);
                    }
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

        public float[] GetObservations(AdventurerAgent agent)
        {
            var shop = shopChooserSubSystem.GetCurrentShop(agent);
            var senseA = shopCraftingSystem.system.shopSubSubSystem.GetSenses(shop);
            var output = new float [1 + AgentShopSubSystem.SenseCount + shopChooserSubSystem.SenseCount];
            output[0] = _currentLocation[agent];
            senseA.CopyTo(output, 1);

            var senseB = shopChooserSubSystem.GetObservations(agent);
            senseB.CopyTo(output, 1 + AgentShopSubSystem.SenseCount);

            return output;
        }
    }
}
