using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using System;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class ToggleButton : MonoBehaviour
    {
        public GameObject craftMenu;

        public GameObject adventurerMenu;

        public bool craftActive;

        private bool showPlayMenus;

		public AdventurerSystemBehaviour adventurerSystemBehaviour;
		public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
		public GameObject BattleUI;
		public bool activeBattle = false;

        private void Start()
        {
            showPlayMenus = Math.Abs(Time.timeScale - 1) < 0.01f;
            UpdateMenu();
        }

        public void SwitchButton()
        {
            craftActive = !craftActive;
			CheckActiveBattle();
			UpdateMenu();
        }

		public void CheckActiveBattle()
		{
			if (getCurrentAdventurerAgent.CurrentAgent == null)
			{
				return;
			}

			//Is the current adventurer agent in battle?
			activeBattle = (bool)(adventurerSystemBehaviour.system.GetAdventureStates(getCurrentAdventurerAgent.CurrentAgent) == AdventureStates.InBattle);
		}

		private void UpdateMenu()
        {
            if (showPlayMenus)
            {
                craftMenu.SetActive(craftActive);
                adventurerMenu.SetActive(!craftActive); 
				
				if(activeBattle == true)
				{
					BattleUI.SetActive(!craftActive);
				}
            }
        }
    }
}
