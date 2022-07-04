using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class ConfirmMenu : MonoBehaviour
    {
        public ConfirmBattleLocationSelect locationSelect;
        public Image[] buttons;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        
        public void Update()
        {
            var battle = locationSelect.GetConfirmation(currentAdventurerAgent.CurrentAgent);
            var index = 0;
            foreach (var i in buttons)
            {
                var highLight = (EConfirmBattle) index == battle;
                i.color = highLight ? Color.green : Color.white;
                index++;
            }
        }
    }
}
