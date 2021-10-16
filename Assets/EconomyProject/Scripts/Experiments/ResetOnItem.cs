using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.Experiments
{
    public class ResetOnItem : MonoBehaviour
    {
        public UsableItem endItem;

        public bool resetOnComplete = false;

        public GameObject agentList;

        private AdventurerAgent[] AdventurerAgents
        {
            get
            {
                return agentList? agentList.GetComponentsInChildren<AdventurerAgent>() : new AdventurerAgent[]{};
            }
        }

        private ShopAgent[] ShopAgents => FindObjectsOfType<ShopAgent>();

        private void Update()
        {
            if (resetOnComplete && endItem)
            {
                foreach (var agent in AdventurerAgents)
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
