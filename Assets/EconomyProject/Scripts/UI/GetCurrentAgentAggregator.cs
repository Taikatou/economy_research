using Data;
using EconomyProject.Scripts.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class GetCurrentAgentAggregator : MonoBehaviour
    {
        public GetCurrentAdventurerAgent CurrentAdventurerAgent;
        public GetCurrentShopAgent CurrentShopAgent;

        public IEconomyAgent CurrentAgent => UISpec.CraftActive ? (IEconomyAgent)CurrentShopAgent.CurrentAgent : 
            (IEconomyAgent)CurrentAdventurerAgent.CurrentAgent;
    }
}
