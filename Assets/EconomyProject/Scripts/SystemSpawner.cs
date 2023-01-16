using EconomyProject.Scripts.MLAgents;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class SystemSpawner : MonoBehaviour
    {
        public int numLearningAgents = 1;
        public int numAgents = 30;
        public GameObject learningAgentPrefab;
        public bool recordDemonstrations = false;

        public void StartSpawn()
        {
            var spawner = GetComponentInChildren<BaseAgentSpawner>();
            var adventurerPrefab = learningAgentPrefab;
            var agents = spawner.SpawnAgents(adventurerPrefab, numAgents);
            
            if (recordDemonstrations)
            {
                var agent = agents[0];
                var recorder = agent.GetComponent<EconomyDemonstrationRecorder>();
                recorder.Record = true;
                
            }
        }
    }
}
