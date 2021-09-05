using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class EndSimulatorUi : MonoBehaviour
    {
        public Text auctionText;

        public EndTimerScript endTimer;
        void Update()
        {
            auctionText.text = "REMAINING: " + endTimer.CurrentTime;
        }
    }
}
