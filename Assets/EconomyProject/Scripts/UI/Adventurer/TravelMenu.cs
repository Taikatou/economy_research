using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using UnityEngine;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class TravelMenu : MonoBehaviour
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        public AdventurerSystemBehaviour adventurerSystem;

        public void ForestBattle()
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.AdventureForest);
        }
        
        public void MountainBattle()
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.AdventureMountain);
        }
        
        public void VolcanoBattle()
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.AdventureVolcano);
        }
        
        public void SeaBattle()
        {
			currentAdventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.AdventureSea);
        }
    }
}
