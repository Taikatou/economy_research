using EconomyProject.Monobehaviours;
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

        public AdventurerSystemBehaviour aSystem;
        
        public void Update()
        {
            var agent = adventurerAgent.CurrentAgent;
            if (agent != null)
            {
                var battle = battleLocationSelect.GetBattleAction(agent);
                var index = 0;
                var subSystem = aSystem.system.battleSubSystem.GetSubSystem(agent);
                if (subSystem != null)
                {
                    var playerData = subSystem.PlayerFighterUnits.GetAgentPlayerData(agent.GetHashCode());
                    var map = playerData.AttackActionMap;
                    
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
}
