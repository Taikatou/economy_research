using System.Collections.Generic;
using Data;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public class BaseAgentSpawner : LastUpdate
    {
        private int _counter = 0;
        public List<GameObject> SpawnAgents(GameObject learningAgentPrefab, int numLearningAgents)
        {
            var output = new List<GameObject>();
            for (var i = 0; i < numLearningAgents; i++)
            {
                var agent = Spawn(learningAgentPrefab);
                var agentId = agent.GetComponent<AgentID>();
                agentId.agentId = _counter;
                _counter++;
                output.Add(agent);
            }

            return output;
        }

        protected virtual GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agentPrefab = Instantiate(toSpawnPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            agentPrefab.transform.parent = gameObject.transform;
            return agentPrefab;
        }
    }
}
