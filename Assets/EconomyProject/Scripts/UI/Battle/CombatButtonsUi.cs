using EconomyProject.Monobehaviours;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Battle
{
    public class CombatButtonsUi : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystem;

        public GetCurrentAdventurerAgent adventurerAgent;

        public void OnAttackButton()
        {
            adventurerSystem.system.OnAttackButton(adventurerAgent.CurrentAgent);
        }

        public void OnHealButton()
        {
            adventurerSystem.system.OnHealButton(adventurerAgent.CurrentAgent);
        }

        public void OnFleeButton()
        {
            adventurerSystem.system.OnFleeButton(adventurerAgent.CurrentAgent);
        }
    }
}
