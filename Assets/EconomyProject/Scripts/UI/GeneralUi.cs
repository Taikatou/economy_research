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

        public GetCurrentAgent<Agent> getAgent;

        public Dropdown dropDown;

        private HashSet<string> _agentIds;

        public AdventurerAgent AdventurerAgent
        {
            get
            {
                var adventurer = getAgent.CurrentAgent.GetComponent<AdventurerAgent>();
                return adventurer;
            }
        }

        private void Start()
        {
            _agentIds = new HashSet<string>();
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
                    durabilityText.text = "DURABILITY: " + (item.unBreakable? "∞" : item.durability.ToString());

                    currentItemText.text = "CURRENT ITEM: " + item.itemName;

                    efficiencyText.text = "EFFICIENCY: " + item.efficiency;
                }
            }

            // todo add this to dropdown text
            /*foreach (var agent in AdventurerAgent.GetAgents)
            {
                string agentId = agent.GetComponent<AgentID>().agentId.ToString();
                if (!_agentIds.Contains(agentId))
                {
                    _agentIds.Add(agentId);
                    dropDown.options.Add(new Dropdown.OptionData(agentId));
                }
            }
            AdventurerAgent.Index = dropDown.value;*/
        }
    }
}
