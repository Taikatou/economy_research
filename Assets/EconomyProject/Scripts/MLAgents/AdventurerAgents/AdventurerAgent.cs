using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.ConfigurationSystem;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
using Inventory;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public class AdventurerAgent : AgentScreen<EAdventurerScreen>, IEconomyAgent
	{
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;
        public AdventurerFighterData fighterData;

        private IEconomyAgent _economyAgentImplementation;

        public override AgentType agentType => AgentType.Adventurer;

        public AdventurerAgentBattleData levelComponent;

        public EAdventurerTypes AdventurerType => levelComponent.adventurerType;

        private static int _idCounter = 0;

        public int agentId;
        
		public override EAdventurerScreen ChosenScreen
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

			agentId = _idCounter;
			_idCounter++;
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
	        return adventurerInput.GetActionMask(this);
        }
        

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

        public void SetAction(int input)
        {
	        _choosenAction = (EAdventurerAgentChoices) input;
        }

        private EAdventurerAgentChoices _choosenAction;

        private static int MaxLevel => 5;
	}
}