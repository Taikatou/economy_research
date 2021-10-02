using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using TurnBased.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Battle
{
    public class CombatButtonsUi : MonoBehaviour
    {
        public BattleLocationSelect battleLocationSelect;

        public GetCurrentAdventurerAgent adventurerAgent;

        public Image[] buttons;

        public void Update()
        {
            var battle = battleLocationSelect.GetBattleAction(adventurerAgent.CurrentAgent);
            var index = 0;
            foreach (var i in buttons)
            {
                var highLight = (EBattleAction) index == battle;
                i.color = highLight ? Color.green : Color.white;
                index++;
            }
        }
    }
}
