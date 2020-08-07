using System;
using EconomyProject.Scripts.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class SystemSpawner : MonoBehaviour
    {
        public int numLearningAgents = 1;
        public GameObject learningAgentPrefab;

        public void Start()
        {
            var spawner = GetComponentInChildren<BaseAgentSpawner>();
            var adventurerPrefab = learningAgentPrefab;
            spawner.SpawnAgents(adventurerPrefab, numLearningAgents);
        }
    }
}
