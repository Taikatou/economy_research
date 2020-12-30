using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class TravelMenu : MonoBehaviour
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        public AdventurerSystemBehaviour adventurerSystem;

        public void ForestBattle()
        {
            adventurerSystem.system.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Forest);
        }
        
        public void MountainBattle()
        {
            adventurerSystem.system.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Mountain);
        }
        
        public void VolcanoBattle()
        {
            adventurerSystem.system.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Volcano);
        }
        
        public void SeaBattle()
        {
            adventurerSystem.system.StartBattle(currentAdventurerAgent.CurrentAgent, BattleEnvironments.Sea);
        }
    }
}
