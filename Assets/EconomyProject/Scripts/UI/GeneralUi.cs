﻿using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI.Adventurer;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class GeneralUi : MonoBehaviour
    {
        public Text durabilityText;

        public Text currentItemText;

        public GetCurrentAdventurerAgent getAgent;

        private AdventurerAgent AdventurerAgent
        {
            get
            {
                AdventurerAgent toReturn = null;
                if (getAgent.CurrentAgent)
                {
                    toReturn = getAgent.CurrentAgent.GetComponent<AdventurerAgent>();
                }
                return toReturn;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if(AdventurerAgent)
            {
                var item = AdventurerAgent.adventurerInventory.EquipedItem;
                if(item)
                {
                    durabilityText.text = "DURABILITY: " + (item.itemDetails.unBreakable? "∞" : item.itemDetails.durability.ToString());

                    currentItemText.text = "CURRENT ITEM: " + item.itemDetails.itemName;
                }
            }
        }
    }
}
