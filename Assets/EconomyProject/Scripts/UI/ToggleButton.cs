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
	        UISpec.craftActive = !UISpec.craftActive;
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
			activeBattle = (bool)(adventurerSystemBehaviour.system.GetAdventureStates(getCurrentAdventurerAgent.CurrentAgent) == EAdventureStates.InBattle);
		}

		private void UpdateMenu()
        {
            if (showPlayMenus)
            {
                craftMenu.SetActive(UISpec.craftActive);
                adventurerMenu.SetActive(!UISpec.craftActive); 
				
				if(activeBattle == true)
				{
					BattleUI.SetActive(!UISpec.craftActive);
				}
            }
        }
    }
}
