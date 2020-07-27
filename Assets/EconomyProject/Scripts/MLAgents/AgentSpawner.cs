using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public class AgentSpawner : BaseAgentSpawner
    {
        public GameObject learningAgentPrefab;

        public MarketPlace marketPlace;

        public int numSpawn;
        private void Start()
        {
            SpawnAgents(learningAgentPrefab, numSpawn);
        }
    }
}
