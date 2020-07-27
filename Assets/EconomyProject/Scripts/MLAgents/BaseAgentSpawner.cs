using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public class BaseAgentSpawner : MonoBehaviour
    {
        public void SpawnAgents(GameObject learningAgentPrefab, int numLearningAgents)
        {
            for (var i = 0; i < numLearningAgents; i++)
            {
                var agentPrefab = Spawn(learningAgentPrefab);
            }
        }

        public virtual GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agentPrefab = Instantiate(toSpawnPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            agentPrefab.transform.parent = gameObject.transform;
            return agentPrefab;
        }
    }
}
