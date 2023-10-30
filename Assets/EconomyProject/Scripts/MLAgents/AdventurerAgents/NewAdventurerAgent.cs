using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using Unity.MLAgents;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class NewAdventurerAgent : Agent, IAdventurerAgent
    {
        public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerRequestTaker requestTaker;
        public AdventurerFighterData fighterData;
        
        public void SetAction(int action)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<EnabledInput> GetEnabledInput()
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void Start()
        {
            if (TrainingConfig.OnPurchase)
            {
                inventory.onItemAdd = OnItemAddReward;
            }
        }
        
        private void OnItemAddReward(UsableItem usableItem)
        {
            if (adventurerInventory)
            {
                if (adventurerInventory.EquipedItem)
                {
                    if (usableItem.itemDetails.damage > adventurerInventory.EquipedItem.itemDetails.damage)
                    {
                        AddReward(TrainingConfig.OnPurchaseReward);
                    }	
                }
            }
        }

        public override void OnEpisodeBegin()
        {
            wallet.Setup(requestTaker.requestSystem, AgentType.Adventurer);
            inventory.Setup();
            fighterData.Setup();
        }

        public AdventurerAgentBattleData LevelComponent => null;
        public EAdventurerTypes AdventurerType => EAdventurerTypes.All;
        public EconomyWallet Wallet => wallet;
    }
}
