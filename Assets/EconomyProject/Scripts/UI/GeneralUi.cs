using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI.Adventurer;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class GeneralUi : MonoBehaviour
    {
        public Text auctionText;

        public Text moneyText;

        public Text durabilityText;

        public Text currentItemText;

        public Text efficiencyText;

        public UiAccessor accessor;

        public GetCurrentAdventurerAgent getAgent;

        private AdventurerAgent AdventurerAgent
        {
            get
            {
                var adventurer = getAgent.CurrentAgent.GetComponent<AdventurerAgent>();
                return adventurer;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            var gameAuction = accessor.GameAuction;
            if (gameAuction)
            {
                auctionText.text = "Auction Items: " + gameAuction.ItemCount;
            }
            if(AdventurerAgent)
            {
                moneyText.text = "MONEY: " + AdventurerAgent.wallet.Money;

                var item = AdventurerAgent.adventurerInventory.EquipedItem;
                if(item)
                {
                    durabilityText.text = "DURABILITY: " + (item.itemDetails.unBreakable? "∞" : item.itemDetails.durability.ToString());

                    currentItemText.text = "CURRENT ITEM: " + item.itemDetails.itemName;

                    efficiencyText.text = "EFFICIENCY: " + item.itemDetails.damage;
                }
            }
        }
    }
}
