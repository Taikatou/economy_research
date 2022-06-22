﻿using System;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerSpawner : BaseAgentSpawner
    {
        public bool randomiseAdventurerType;
        public AdventurerInput adventurerInput;

        public RequestSystem requestSystem;

        public CraftingRequestRecord requestRecord;
        
        protected override GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agent = base.Spawn(toSpawnPrefab);
            var adventurer = agent.GetComponent<AdventurerAgent>();
            adventurer.adventurerInput = adventurerInput;
            if (randomiseAdventurerType)
            {
                var values = Enum.GetValues(typeof(EAdventurerTypes));
                var random = new System.Random();
                var battleData = agent.GetComponent<AdventurerAgentBattleData>();
                battleData.adventurerType = (EAdventurerTypes) values.GetValue(random.Next(1, values.Length));
            }

            var taker = adventurer.GetComponent<AdventurerRequestTaker>();
            taker.requestSystem = requestSystem;
            taker.requestRecord = requestRecord;
            return agent;
        }
        
        
    }
}
