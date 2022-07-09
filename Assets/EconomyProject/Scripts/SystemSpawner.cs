﻿using EconomyProject.Scripts.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class SystemSpawner : MonoBehaviour
    {
        public int numLearningAgents = 1;
        public int numAgents = 30;
        public GameObject learningAgentPrefab;

        public void StartSpawn()
        {
            var spawner = GetComponentInChildren<BaseAgentSpawner>();
            var adventurerPrefab = learningAgentPrefab;
            spawner.SpawnAgents(adventurerPrefab, numAgents);
        }
    }
}
