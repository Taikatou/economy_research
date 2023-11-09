using System;
using System.Globalization;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Battle
{
    public class BattleTimerText : MonoBehaviour
    {
        public Text text;
        public AdventurerSystemBehaviour adventurerSystem;
        public GetCurrentAdventurerAgent adventurerAgent;
        
        private BattleSubSystemInstance<BaseAdventurerAgent> GetAdventurerBattleSubsystem()
        {
            var agent = adventurerAgent.CurrentAgent;
            var system = adventurerSystem.system.GetAdventureStates(agent);
            if (system == EAdventureStates.InBattle)
            {
                return adventurerSystem.system.battleSubSystem.GetSubSystem(agent);
            }

            return null;
        }

        public void Update()
        {
            var system = GetAdventurerBattleSubsystem();
            if (system != null)
            {
                text.text = system.CurrentTimerValue.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
