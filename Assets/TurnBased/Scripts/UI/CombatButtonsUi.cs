using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.UI;
using UnityEngine;

namespace TurnBased.Scripts.UI
{
    public class CombatButtonsUi : MonoBehaviour
    {
        public AdventurerSystem adventurerSystem;

        public GetCurrentAdventurerAgent adventurerAgent;

        public void OnAttackButton()
        {
            adventurerSystem.OnAttackButton(adventurerAgent.CurrentAgent);
        }

        public void OnHealButton()
        {
            adventurerSystem.OnHealButton(adventurerAgent.CurrentAgent);
        }
    }
}
