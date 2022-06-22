using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using System;
using Data;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class ToggleButton : MonoBehaviour
    {
        public GameObject craftMenu;

        public GameObject adventurerMenu;

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
	        UISpec.CraftActive = !UISpec.CraftActive;
			CheckActiveBattle();
			UpdateMenu();
        }

        private void CheckActiveBattle()
		{
			if (getCurrentAdventurerAgent.CurrentAgent == null)
			{
				return;
			}

			//Is the current adventurer agent in battle?
			activeBattle = (bool)(adventurerSystemBehaviour.system.GetAdventureStates(getCurrentAdventurerAgent.CurrentAgent) == EAdventureStates.InBattle);
		}

		private void UpdateMenu()
        {
            if (showPlayMenus && craftMenu != null && adventurerMenu != null)
            {
                craftMenu.SetActive(UISpec.CraftActive);
                adventurerMenu.SetActive(!UISpec.CraftActive); 
				
				if(activeBattle == true)
				{
					BattleUI.SetActive(!UISpec.CraftActive);
				}
            }
        }
    }
}
