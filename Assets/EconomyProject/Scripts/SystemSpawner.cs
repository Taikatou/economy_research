using System;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class SystemSpawner : MonoBehaviour
    {
        public int numLearningAgents = 1;
        public GameObject learningAgentPrefab;

        public void StartSpawn()
        {
            var spawner = GetComponentInChildren<BaseAgentSpawner>();
            var adventurerPrefab = learningAgentPrefab;
            spawner.SpawnAgents(adventurerPrefab, numLearningAgents);
        }
    }
}
