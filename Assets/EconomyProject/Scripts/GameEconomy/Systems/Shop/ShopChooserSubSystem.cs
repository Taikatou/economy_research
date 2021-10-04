using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public delegate void OnShopChange(int index);
    [Serializable]
    public class ShopChooserSubSystem : GetCurrentShopAgent, IMoveMenu<AdventurerAgent>
    {
        private Dictionary<AdventurerAgent, int> _currentShop;

        public OnShopChange onShopChange;
        
        public static readonly int SensorCount = 2;
        
        public void Start()
        {
            _currentShop = new Dictionary<AdventurerAgent, int>();
        }

        public ShopAgent GetCurrentShop(AdventurerAgent agent)
        {
			GetCurrentShopAgent getShopAgent = FindObjectOfType<GetCurrentShopAgent>();
			return getShopAgent.CurrentAgent;
		}

		public void MovePosition(AdventurerAgent agent, int movement)
        {
            var newPosition = GetAction(agent) + movement;
            ChangePosition(agent, newPosition);
        }

        public int GetAction(AdventurerAgent agent)
        {
            if (!_currentShop.ContainsKey(agent))
            {
                _currentShop.Add(agent, 0);
            }

            return _currentShop[agent];
        }

        private void ChangePosition(AdventurerAgent agent, int newPosition)
        {
            if (newPosition < GetAgents.Length && newPosition >= 0)
            {
                _currentShop[agent] = newPosition;
                onShopChange?.Invoke(newPosition);
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
            var shop = GetAction(agent);
            return new float [] { _currentShop.Count, shop };
        }
    }
}
