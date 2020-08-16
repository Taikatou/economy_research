using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class TravelMenu : MonoBehaviour
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        public AdventurerSystem adventurerSystem;

        public void ForestBattle()
        {
            adventurerSystem.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Forest);
        }
        
        public void MountainBattle()
        {
            adventurerSystem.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Mountain);
        }
        
        public void VolcanoBattle()
        {
            adventurerSystem.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Volcano);
        }
        
        public void SeaBattle()
        {
            adventurerSystem.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Sea);
        }
    }
}
