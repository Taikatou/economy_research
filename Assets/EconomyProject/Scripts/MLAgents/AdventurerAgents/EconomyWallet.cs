using System;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class EconomyWallet : MonoBehaviour
    {
        public double startMoney;

        public double EarnedMoney { get; private set; }
        public double SpentMoney { get; private set; }
        public double Money { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Money = startMoney;
        }

        public void EarnMoney(double amount)
        {
            if (amount > 0)
            {
                Money = Math.Round(Money + amount);
                EarnedMoney += amount;
            }
        }

        public void LoseMoney(double amount)
        {
            if (amount < 0)
            {
                Money = Math.Round(Money + amount);
                SpentMoney += amount;
            }
        }

        public void SpendMoney(double amount)
        {
            if (amount > 0)
            {
                Money -= amount;
            }
        }

        public void SetMoney(double amount)
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
