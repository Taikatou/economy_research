using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public enum NewAdventurerScreen {Main}
    public class NewAdventurerAgent : BaseAdventurerAgent, IScreenSelect<NewAdventurerScreen>, IEconomyAgent
    {
        public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerRequestTaker requestTaker;
        public AdventurerFighterData fighterData;
        
        public void SetAction(int action)
        {
            _choosenAction = (ENewAdventurerAgentChoices) action;
        }

        public IEnumerable<EnabledInput> GetEnabledInput()
        {
            throw new NotImplementedException();
        }

        private ENewAdventurerAgentChoices _choosenAction;

        public List<EnabledInput[]> GetEnabledInputNew()
        {
            var advInputs = AdventurerInput.AdventurerSystem.system.GetEnabledInputs(this, 0);
            var shopInputs = AdventurerInput.AdventurerShopSystem.system.GetEnabledInputs(this, 1);
            return new List<EnabledInput[]>
            {
                advInputs,
                shopInputs
            };
        }

        public int HalfSize => 4;

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
        
        public override void OnActionReceived(ActionBuffers actions)
        {
            var battleAction = (EAdventurerAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);
            if(battleAction != EAdventurerAgentChoices.None)
            {
                AdventurerInput.AdventurerSystem.system.AgentSetChoice(this, battleAction, 0);
            }
            
            var shopAction = (EAdventurerAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[1]);
            if (shopAction != EAdventurerAgentChoices.None)
            {
                AdventurerInput.AdventurerShopSystem.system.AgentSetChoice(this, shopAction, 1);
            }
        }
        
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            var i = 0;
            foreach (var sysInputs in GetEnabledInputNew())
            {
                foreach (var input in sysInputs)
                {
                    actionMask.SetActionEnabled(i, input.Input, input.Enabled);
                }
                i++;
            }
        }
        
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            if (_choosenAction != ENewAdventurerAgentChoices.None)
            {
                var actions = actionsOut.DiscreteActions;
                if (_choosenAction <= ENewAdventurerAgentChoices.AdvSelect)
                {
                    actions[1] = (int)_choosenAction;
                }
                else
                {
                    actions[0] = (int)ChoiceMaps[_choosenAction];
                }
                _choosenAction = ENewAdventurerAgentChoices.None;
            }
        }

        public static readonly Dictionary<ENewAdventurerAgentChoices, ENewAdventurerAgentChoices> ChoiceMaps =
            new()
            {
                { ENewAdventurerAgentChoices.ShopBack, ENewAdventurerAgentChoices.AdvBack },
                { ENewAdventurerAgentChoices.ShopDown, ENewAdventurerAgentChoices.AdvDown },
                { ENewAdventurerAgentChoices.ShopUp, ENewAdventurerAgentChoices.AdvUp },
                { ENewAdventurerAgentChoices.ShopSelect, ENewAdventurerAgentChoices.AdvSelect },
                { ENewAdventurerAgentChoices.ShopNone, ENewAdventurerAgentChoices.None }
            };

        public override AdventurerAgentBattleData LevelComponent => null;
        public override EAdventurerTypes AdventurerType => EAdventurerTypes.All;
        public override EconomyWallet Wallet => wallet;
        public override AdventurerInventory AdventurerInventory => adventurerInventory;
        public override AgentInventory Inventory => inventory;
        public override AdventurerRequestTaker RequestTaker => requestTaker;
        public override AdventurerFighterData FighterData => fighterData;
        public override int ChosenScreenInt => (int)ChosenScreen;
        public override AgentType agentType => AgentType.Adventurer;
        public NewAdventurerScreen ChosenScreen =>  NewAdventurerScreen.Main;
    }
}
