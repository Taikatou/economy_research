using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class AdventurerUISelection : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public Text dropDown;
        public EBattleEnvironments environment;

        public void Update()
        {
            var players = "";
            var currentParties = adventurerSystem.system.battleSubSystem.CurrentParties;
            if(currentParties.ContainsKey(environment))
            {
                foreach (var agent in currentParties[environment].PendingAgents)
                {
                    players += "ABCD\t" +  agent.AdventurerType + "\t" + agent.levelComponent.Level + "\n";
                }
            }

            dropDown.text = players;
        }
    }
}
