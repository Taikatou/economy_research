using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using TurnBased.Scripts;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public abstract class BaseAdventurerAgent : AgentScreen
	{
		public abstract AdventurerAgentBattleData LevelComponent { get; }
		public abstract EAdventurerTypes AdventurerType { get; }
		public abstract EconomyWallet Wallet { get; }
		public abstract AdventurerInventory AdventurerInventory { get; }
		public abstract AgentInventory Inventory { get; }
		public abstract AdventurerRequestTaker RequestTaker { get; }
		public abstract AdventurerFighterData FighterData { get; }
	}
	public class AdventurerAgent : BaseAdventurerAgent, IEconomyAgent, IScreenSelect<EAdventurerScreen>
	{
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;
        public AdventurerFighterData fighterData;
        public override int ChosenScreenInt => (int)ChosenScreen;
        public override AgentType agentType => AgentType.Adventurer;

        public AdventurerAgentBattleData levelComponent;


        public EAdventurerScreen ChosenScreen
		{
			get
			{
				var toReturn = EAdventurerScreen.Adventurer;
				if (adventurerInput)
				{
					toReturn = adventurerInput.GetScreen(this, TrainingConfig.StartScreen);
				}
				return toReturn;
			}
		}

		private void OnLevelUp(int level)
		{
			var dataLogger = FindObjectOfType<LevelDataLogger>();
			if (dataLogger != null)
			{ 
				dataLogger.AddLevelData(level, AdventurerType, StepCount);
			}

			if (TrainingConfig.OnLevelUp)
			{
				Debug.Log("Add reward level up");
				AddReward(TrainingConfig.OnLevelUpReward);
			}

			if (level == MaxLevel)
			{
				EndEpisode();
			}
		}

		public virtual void Start()
		{
			levelComponent.OnLevelUp = OnLevelUp;
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

			if (usableItem.craftChoice == ECraftingChoice.UltimateSwordOfPower)
			{
				EndEpisode();
			}
		}

		public override void OnEpisodeBegin()
        {
	        levelComponent.Reset();
	        wallet.Setup(requestTaker.requestSystem, AgentType.Adventurer);
	        inventory.Setup();
	        fighterData.Setup();

	        var adventurerBehaviour = FindObjectOfType<AdventurerSystemBehaviour>();
	        adventurerBehaviour.system.RemoveAgent(this);
		
	        ResetOnItem.bSetupSystems = true;
        }

		public override void Heuristic(in ActionBuffers actionsOut)
        {
	        if (_choosenAction != EAdventurerAgentChoices.None)
	        {
		        var actions = actionsOut.DiscreteActions;
		        actions[0] = (int)_choosenAction;
		        _choosenAction = EAdventurerAgentChoices.None;
	        }
        }
        
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
	        foreach (var input in GetEnabledInput())
	        {
		        actionMask.SetActionEnabled(0, input.Input, input.Enabled);   
	        }
        }

        public IEnumerable<EnabledInput> GetEnabledInput()
        {
	        return new EnabledInput[] { };
        }

        public List<EnabledInput[]> GetEnabledInputNew()
        {
	        throw new NotImplementedException();
        }

        public int HalfSize { get; }


        public override void OnActionReceived(ActionBuffers actions)
        {
	        var action = (EAdventurerAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);;
	        ChooseAction(action);
        }

        private void ChooseAction(EAdventurerAgentChoices action)
        {
	        if (action != EAdventurerAgentChoices.None)
	        {
		        var system = adventurerInput.GetEconomySystem(this);
		        system.AgentSetChoice(this, action);
		        if (TrainingConfig.PunishMovement)
		        {
			        AddReward(TrainingConfig.OnPunishMovementReward);   
		        }
	        }
        }

        public void SetAction(int input, bool shop)
        {
	        _choosenAction = (EAdventurerAgentChoices) input;
        }

        private EAdventurerAgentChoices _choosenAction;

        private static int MaxLevel => 5;
        public override AdventurerAgentBattleData LevelComponent => levelComponent;
        public override EAdventurerTypes AdventurerType => EAdventurerTypes.All;
        public override EconomyWallet Wallet => wallet;
        public override AdventurerInventory AdventurerInventory => adventurerInventory;
        public override AgentInventory Inventory => inventory;
        public override AdventurerRequestTaker RequestTaker => requestTaker;
        public override AdventurerFighterData FighterData => fighterData;
	}
}