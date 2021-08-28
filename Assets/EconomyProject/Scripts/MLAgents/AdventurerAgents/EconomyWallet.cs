using System;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class EconomyWallet : MonoBehaviour
    {
        public int startMoney;
        private int EarnedMoney { get; set; }
        private int SpentMoney { get; set; }
        public int Money { get; private set; }
		
		void Start()
        {
			Reset();
		}

		private AgentType GetAgentType()
		{
			if (GetComponent<AdventurerAgent>() != null)
			{
				return AgentType.Adventurer;
			}
			if (GetComponent<ShopAgent>() != null)
			{
				return AgentType.Shop;
			}

			return AgentType.None;
		}

		public void EarnMoney(int amount)
        {
            if (amount > 0)
            {
                Money += amount;
                EarnedMoney += amount;
            }
        }

        public void LoseMoney(int amount)
        {
            if (amount > 0)
            {
                Money -= amount;
			}
			else
			{
				Money += amount;
			}
			if(Money < 0)
			{
				Money = 0;
			}
        }

        public void SpendMoney(int amount)
        {
            if (amount > 0)
            {
                Money -= amount;
				SpentMoney += amount;
			}
        }

        public void SetMoney(int amount)
        {
            Money = amount;
        }

        public void Reset()
        {
			var requestSystem = FindObjectOfType<RequestSystem>();
			var agentType = GetAgentType();
			if (requestSystem == null || agentType == AgentType.None)
			{
				throw new Exception();
			}
			if (!requestSystem.StartMoney.ContainsKey(agentType))
			{
				throw new Exception();
			}
			startMoney = requestSystem.StartMoney[agentType];
			Money = startMoney;
        }

        public void ResetStep()
        {
            EarnedMoney = 0;
            SpentMoney = 0;
        }
    }
}
