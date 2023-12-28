using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
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
        
        public void SetAction(int action, bool isShop)
        {
            _choosenAction = (EAdventurerAgentChoices) action;
            _branch = isShop ? 1 : 0;
            RequestDecision();
        }

        private BehaviorParameters _behaviorParameters;

        public IEnumerable<EnabledInput> GetEnabledInput()
        {
            var toReturn = new List<EnabledInput>();
            if (_behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
            {
                var actions = GetEnabledActions();
                foreach (var input in ValuesAsArray)
                {
                    var enabled = actions[0].Contains(input) || actions[1].Contains(input);
                    var enabledInput = new EnabledInput
                    {
                        Enabled = enabled,
                        Input = (int)input
                    };
                    toReturn.Add(enabledInput);
                }
            }

            return toReturn;
        }

        private EAdventurerAgentChoices _choosenAction;
        private int _branch;
        public int HalfSize => 4;

        public virtual void Start()
        {
            if (TrainingConfig.OnPurchase)
            {
                inventory.onItemAdd = OnItemAddReward;
            }

            _behaviorParameters = GetComponent<BehaviorParameters>();
        }
        
        private void OnItemAddReward(UsableItem usableItem)
        {
            if (adventurerInventory)
            {
                if (adventurerInventory.EquipedItem)
                {
                    if (usableItem.itemDetails.damage > adventurerInventory.EquipedItem.itemDetails.damage)
                    {
                        AddReward(TrainingConfig.OnPurchaseReward * ((float)usableItem.itemDetails.durability/20));
                    }	
                }

                if (usableItem.craftChoice == ECraftingChoice.UltimateSwordOfPower)
                {
                    EndEpisode();
                }
            }
        }
        
        public override void OnEpisodeBegin()
        {
            wallet.Setup(requestTaker.requestSystem, AgentType.Adventurer);
            inventory.Setup();
            fighterData.Setup();
        }
        
        readonly Dictionary<EAdventurerAgentChoices, EBattleEnvironments> _selectLocation = new Dictionary<EAdventurerAgentChoices, EBattleEnvironments>
        {
            { EAdventurerAgentChoices.SelectForest, EBattleEnvironments.Forest },
            { EAdventurerAgentChoices.SelectMountain, EBattleEnvironments.Mountain },
            { EAdventurerAgentChoices.SelectSea, EBattleEnvironments.Sea },
            { EAdventurerAgentChoices.SelectVolcano, EBattleEnvironments.Volcano }
        };
        
        readonly Dictionary<EAdventurerAgentChoices, ECraftingChoice> _buyItem = new Dictionary<EAdventurerAgentChoices, ECraftingChoice>
        {
            { EAdventurerAgentChoices.BuyBeginnerSword, ECraftingChoice.BeginnerSword },
            { EAdventurerAgentChoices.BuyIntermediateSword, ECraftingChoice.IntermediateSword },
            { EAdventurerAgentChoices.BuyAdvancedSword, ECraftingChoice.AdvancedSword },
            { EAdventurerAgentChoices.BuyEpicSword, ECraftingChoice.EpicSword },
            { EAdventurerAgentChoices.BuyUltimateSword, ECraftingChoice.UltimateSwordOfPower },
            { EAdventurerAgentChoices.BuyMasterSword, ECraftingChoice.MasterSword }
        };
        
        public override void OnActionReceived(ActionBuffers actions)
        {
            var battleAction = (EAdventurerAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);
            if (battleAction is >= EAdventurerAgentChoices.SelectForest and <= EAdventurerAgentChoices.SelectVolcano)
            {
                AdventurerInput.AdventurerSystem.system.SelectOutOfBattle(this, _selectLocation[battleAction]);
            }
            else if (battleAction is >= EAdventurerAgentChoices.BuyBeginnerSword
                     and <= EAdventurerAgentChoices.BuyUltimateSword)
            {
                AdventurerInput.AdventurerShopSystem.system.adventurerShopSubSystem.PurchaseItem(this, _buyItem[battleAction]);
            }
        }
        private static readonly EAdventurerAgentChoices [] ValuesAsArray
            = Enum.GetValues(typeof(EAdventurerAgentChoices)).Cast<EAdventurerAgentChoices>().ToArray();

        public List<EAdventurerAgentChoices[]> GetEnabledActions()
        {
            var advInputs = AdventurerInput.AdventurerSystem.system.GetEnabledInputs(this);
            var shopInputs = AdventurerInput.AdventurerShopSystem.system.GetEnabledInputs(this);
            return new List<EAdventurerAgentChoices[]> { advInputs, shopInputs };
        }
        
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            var actions = GetEnabledActions();
            
            foreach (var input in ValuesAsArray)
            {
                var enabled = actions[0].Contains(input) || actions[1].Contains(input);
                actionMask.SetActionEnabled(0, (int) input, enabled);
            }
        }
        
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            if (_choosenAction != EAdventurerAgentChoices.None)
            {
                var actions = actionsOut.DiscreteActions;

                actions[_branch] = (int)_choosenAction;
                _choosenAction = EAdventurerAgentChoices.None;
            }
        }

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
