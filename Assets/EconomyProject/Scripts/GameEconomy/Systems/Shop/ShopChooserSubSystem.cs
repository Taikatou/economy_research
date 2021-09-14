using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    [Serializable]
    public class ShopChooserSubSystem : GetCurrentShopAgent, IShopSubSystem, IAdventureSense
    {
        private Dictionary<AdventurerAgent, int> _currentShop;

        public int SenseCount => 1;
        
        public void Start()
        {
            _currentShop = new Dictionary<AdventurerAgent, int>();
        }

        public void SetInput(AdventurerAgent agent, EAdventureShopChoices choice)
        {
            GetCurrentShop(agent);

            switch (choice)
            {
                case EAdventureShopChoices.Up:
                    MovePosition(agent, 1);
                    break;
                
                case EAdventureShopChoices.Down:
                    MovePosition(agent, -1);
                    break;
                
                case EAdventureShopChoices.Select:
                    break;
            }
        }

        public ShopAgent GetCurrentShop(AdventurerAgent agent)
        {
			GetCurrentShopAgent getShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
			return getShopAgent.CurrentAgent;
		}

		private void MovePosition(AdventurerAgent agent, int movement)
        {
            var newPosition = _currentShop[agent] + movement;
            ChangePosition(agent, newPosition);
        }

        private void ChangePosition(AdventurerAgent agent, int newPosition)
        {
            if (newPosition < GetAgents.Length && newPosition >= 0)
            {
                _currentShop[agent] = newPosition;
            }
        }

        public void SetShopAgent(AdventurerAgent adventurerAgent, ShopAgent shopAgent)
        {
            var found = false;
            for (var i = 0; i < GetAgents.Length && !found; i++)
            {
                if (GetAgents[i] == shopAgent)
                {
                    found = true;
                    _currentShop[adventurerAgent] = i;
                }
            }
        }

        public float[] GetObservations(AdventurerAgent agent)
        {
            return new float [] {_currentShop.Count};
        }
    }
}
