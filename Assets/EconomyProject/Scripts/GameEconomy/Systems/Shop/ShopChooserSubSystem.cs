using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class ShopChooserSubSystem : GetCurrentAgent<ShopAgent>, IShopSubSystem
    {
        private Dictionary<AdventurerAgent, int> _currentShop;

        public void Start()
        {
            _currentShop = new Dictionary<AdventurerAgent, int>();
        }

        public void SetInput(AdventurerAgent agent, AdventureShopInput choice)
        {
            GetCurrentShop(agent);

            switch (choice)
            {
                case AdventureShopInput.Up:
                    MoveInput(agent, 1);
                    break;
                
                case AdventureShopInput.Down:
                    MoveInput(agent, -1);
                    break;
                
                case AdventureShopInput.Select:
                    break;
            }
        }

        public ShopAgent GetCurrentShop(AdventurerAgent agent)
        {
            if (!_currentShop.ContainsKey(agent))
            {
                _currentShop.Add(agent, 0);
            }

            return GetAgents[_currentShop[agent]];
        }

        private void MoveInput(AdventurerAgent agent, int movement)
        {
            var newPosition = _currentShop[agent] + movement;
            if (newPosition < GetAgents.Length && newPosition >= 0)
            {
                _currentShop[agent] = newPosition;
            }
        }
    }
}
