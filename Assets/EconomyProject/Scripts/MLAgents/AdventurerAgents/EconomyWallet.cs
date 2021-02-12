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
			if (this.GetComponent<AdventurerAgent>() != null)
			{
				return AgentType.Adventurer;
			}
			else if (this.GetComponent<ShopAgent>() != null)
			{
				return AgentType.Shop;
			}
			else
			{
				Debug.LogError("No agent attached to this wallet : " + this);
				return 0;
			}
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
			RequestSystem requestSystem = GameObject.FindObjectOfType<RequestSystem>();
			startMoney = requestSystem._startMoney[GetAgentType()];
			Money = startMoney;
        }

        public void ResetStep()
        {
            EarnedMoney = 0;
            SpentMoney = 0;
        }
    }
}
