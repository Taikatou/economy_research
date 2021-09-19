using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : MonoBehaviour, IAdventureSense, IMoveMenu<AdventurerAgent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        private Dictionary<AdventurerAgent, int> _currentLocation;

        public void Start()
        {
            _currentLocation = new Dictionary<AdventurerAgent, int>();
        }

        public int GetCurrentLocation(AdventurerAgent agent)
        {
            if (!_currentLocation.ContainsKey(agent))
            {
                _currentLocation.Add(agent, 0);
            }

            return _currentLocation[agent];
        }

        public void PurchaseItem(AdventurerAgent agent)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
            if (_currentLocation[agent] < shopItems.Count)
            {
                var shopDetails = shopItems[_currentLocation[agent]].itemDetails;
                shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(shopAgent, shopDetails, agent.wallet, agent.inventory);
            }
        }

        public void MovePosition(AdventurerAgent agent, int movement)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
            
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
