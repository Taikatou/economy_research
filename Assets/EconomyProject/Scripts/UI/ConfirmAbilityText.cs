using EconomyProject.Monobehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class ConfirmAbilityText : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public GetCurrentAdventurerAgent adventurerAgent;
        public Text textBox;
        void Update()
        {
            var currentParties = adventurerSystem.system.battleSubSystem.GetSubsystem(adventurerAgent.CurrentAgent);
            if (currentParties != null)
            {
                var players = "";
                foreach (var agent in currentParties.PendingAgents)
                {
                    players += agent.AdventurerType + "\t" + agent.levelComponent.Level + "\n";
                }

                textBox.text = players;
            }
        }
    }
}
