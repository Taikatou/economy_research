using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.Experiments
{
    public class ResetOnItem : MonoBehaviour
    {
        public InventoryItem endItem;

        public bool resetOnComplete = false;

        public GameObject agentList;

        private AdventurerAgent[] Agents
        {
            get
            {
                return agentList? agentList.GetComponentsInChildren<AdventurerAgent>() : new AdventurerAgent[]{};
            }
        }

        private void Update()
        {
            if (resetOnComplete && endItem)
            {
                foreach (var agent in Agents)
                {
                    var hasEndItem = agent.inventory.ContainsItem(endItem);
                    if (hasEndItem)
                    {
                        Debug.Log("Complete");
                        agent.AddReward(1);
                        agent.EndEpisode();
                    }
                }
            }
        }
    }
}
