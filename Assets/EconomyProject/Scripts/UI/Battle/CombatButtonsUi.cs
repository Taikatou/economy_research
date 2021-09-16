using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Battle
{
    public class CombatButtonsUi : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;

        public GetCurrentAdventurerAgent adventurerAgent;

        public void OnAttackButton()
        {
			adventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.BAttack);
        }

        public void OnHealButton()
        {
			adventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.BHeal);
        }

        public void OnFleeButton()
        {
			adventurerAgent.CurrentAgent.SetAction(EAdventurerAgentChoices.BFlee);
        }
    }
}
