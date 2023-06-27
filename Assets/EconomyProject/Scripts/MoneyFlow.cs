using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class MoneyFlow : MonoBehaviour
    {
        public GameObject walletParent;
        private float _currentTime;
        public int distributeEvery;
        public int amount = 1;

        // Update is called once per frame
        private void FixedUpdate()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                _currentTime = distributeEvery;
                var wallets = walletParent.GetComponentsInChildren<EconomyWallet>();
                foreach (var wal in wallets)
                {
                    wal.EarnMoney(amount, false);
                }
            }
        }
    }
}
