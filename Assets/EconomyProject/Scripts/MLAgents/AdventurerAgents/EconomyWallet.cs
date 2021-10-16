using System;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public delegate void OnEarnMoney(float income);
    public class EconomyWallet : MonoBehaviour
    {
	    public OnEarnMoney onEarnMoney;
        public int startMoney;
        private int EarnedMoney { get; set; }
        private int SpentMoney { get; set; }
        public int Money { get; private set; }

        public void EarnMoney(int amount)
        {
            if (amount > 0)
            {
                Money += amount;
                EarnedMoney += amount;
                onEarnMoney?.Invoke(amount);
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

        public void Setup(RequestSystem requestSystem, AgentType agentType)
        {
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
    }
}
