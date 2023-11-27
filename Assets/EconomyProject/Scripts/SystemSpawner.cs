using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Demonstrations;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class SystemSpawner : MonoBehaviour
    {
        public int numLearningAgents = 1;
        public int numAgents = 30;
        public GameObject learningAgentPrefab;
        public bool recordDemonstrations = true;
        
        public AdventurerInput adventurerInput;

        public void StartSpawn()
        {
            var spawner = GetComponentInChildren<BaseAgentSpawner>();
            var adventurerPrefab = learningAgentPrefab;
            var agents = spawner.SpawnAgents(adventurerPrefab, numAgents);
            
            if (recordDemonstrations)
            {
                var agentObject = agents[0];

                var agent = agentObject.GetComponent<BaseAdventurerAgent>();
                if (agent != null)
                {
                    adventurerInput.ChangeScreen(agent, TrainingConfig.StartScreen);
                }
            }
        }
    }
}
