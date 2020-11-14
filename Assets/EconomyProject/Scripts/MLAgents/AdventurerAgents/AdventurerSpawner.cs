using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerSpawner : BaseAgentSpawner
    {
        public AdventurerInput adventurerInput;

        public RequestSystem requestSystem;

        public CraftingRequestRecord requestRecord;
        
        protected override GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agent = base.Spawn(toSpawnPrefab);
            var adventurer = agent.GetComponent<AdventurerAgent>(); 
            adventurer.adventurerInput = adventurerInput;

            var taker = adventurer.GetComponent<AdventurerRequestTaker>();
            taker.requestSystem = requestSystem;
            taker.requestRecord = requestRecord;
            return agent;
        }
    }
}
