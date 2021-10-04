using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class TravelMenu : MonoBehaviour
    {
	    public AdventurerLocationSelect getLocationSelect;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        public Image[] buttons;

        public void Update()
        {
	        var battle = getLocationSelect.GetBattle(currentAdventurerAgent.CurrentAgent);
	        var index = 0;
	        foreach (var i in buttons)
	        {
		        var highLight = (EBattleEnvironments) index == battle;
		        i.color = highLight ? Color.green : Color.white;
		        index++;
	        }
        }
    }
}
