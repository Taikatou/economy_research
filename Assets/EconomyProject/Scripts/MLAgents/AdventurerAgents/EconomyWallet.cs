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
	    public OnEarnMoney onLoseMoney;
        public int startMoney;
        private int EarnedMoney { get; set; }
        private int SpentMoney { get; set; }
        public int Money { get; private set; }

        public void EarnMoney(int amount, bool earnedMoney)
        {
            if (amount > 0)
            {
                Money += amount;
                EarnedMoney += amount;
                if (earnedMoney)
                {
	                onEarnMoney?.Invoke(amount);   
                }
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
			onLoseMoney?.Invoke(amount);
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
		        return;
	        }
			if (!RequestSystem.StartMoney.ContainsKey(agentType))
			{
				throw new Exception();
			}
			startMoney = RequestSystem.StartMoney[agentType];
			Money = startMoney;
        }
    }
}
