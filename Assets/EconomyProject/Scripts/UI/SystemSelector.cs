using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class SystemSelector : MonoBehaviour
    {
        public AdventurerSystemLocationSelect getLocationSelect;
        public Image[] buttons;
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        private void Update()
        {
            var battle = getLocationSelect.GetEnvironment(currentAdventurerAgent.CurrentAgent);
            var index = 0;
            foreach (var i in buttons)
            {
                var highLight = (EAdventurerSystem) index == battle;
                i.color = highLight ? Color.green : Color.white;
                index++;
            }
        }
    }
}
