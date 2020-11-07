using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public class BaseAgentSpawner : LastUpdate
    {
        private int _counter = 0;
        public void SpawnAgents(GameObject learningAgentPrefab, int numLearningAgents)
        {
            for (var i = 0; i < numLearningAgents; i++)
            {
                var agent = Spawn(learningAgentPrefab);
                var agentId = agent.GetComponent<AgentID>();
                agentId.agentId = _counter;
                _counter++;
            }
        }

        protected virtual GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agentPrefab = Instantiate(toSpawnPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            agentPrefab.transform.parent = gameObject.transform;
            return agentPrefab;
        }
    }
}
