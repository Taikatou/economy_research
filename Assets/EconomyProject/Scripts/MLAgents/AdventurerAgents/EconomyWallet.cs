using System;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class EconomyWallet : MonoBehaviour
    {
        public int startMoney;
        private int EarnedMoney { get; set; }
        private int SpentMoney { get; set; }
        public int Money { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Money = startMoney;
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
            if (amount < 0)
            {
                Money -= amount;
                SpentMoney += amount;
            }
        }

        public void SpendMoney(int amount)
        {
            if (amount > 0)
            {
                Money -= amount;
            }
        }

        public void SetMoney(int amount)
        {
            Money = amount;
        }

        public void Reset()
        {
            Money = startMoney;
        }

        public void ResetStep()
        {
            EarnedMoney = 0;
            SpentMoney = 0;
        }
    }
}
