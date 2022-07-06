using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using TurnBased.Scripts;
using TurnBased.Scripts.AI;
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
            var agent = adventurerAgent.CurrentAgent;
            if (agent != null)
            {
                var battle = battleLocationSelect.GetBattleAction(agent);
                var index = 0;
                var map = PlayerActionMap.GetAttackActionMap(agent.AdventurerType, PlayerActionMap.GetAbilities(agent.AdventurerType, agent.levelComponent.Level));
                foreach (var i in buttons)
                {
                    var battleAction = (EBattleAction) index;
                    var highLight = battleAction == battle;
                    i.color = highLight ? Color.green : Color.white;
                    index++;

                    var txt = "";
                    if (map.ContainsKey(battleAction))
                    {
                        txt = map[battleAction].ToString();
                    }
                    var text = i.GetComponentInChildren<Text>();
                    text.text = txt;
                }   
            }
        }
    }
}
