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

        private EAdventurerAgentChoices _forcedAction;
        private bool _bForcedAction;
        private IEconomyAgent _economyAgentImplementation;

        public override AgentType agentType => AgentType.Adventurer;

        public AdventurerAgentBattleData levelComponent;

        public EAdventurerTypes AdventurerType => levelComponent.adventurerType;
        
		public override EAdventurerScreen ChosenScreen
		{
			get
			{
				var toReturn = EAdventurerScreen.Adventurer;
				if (adventurerInput)
				{
					toReturn = adventurerInput.GetScreen(this, EAdventurerScreen.Adventurer);
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
			AddReward((float)level/10);
		}

		public void Start()
		{
			levelComponent.OnLevelUp = OnLevelUp + levelComponent.OnLevelUp;
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
	        levelComponent.Reset();

	        var adventurerBehaviour = FindObjectOfType<AdventurerSystemBehaviour>();
	        adventurerBehaviour.system.RemoveAgent(this);
		
	        ResetOnItem.bSetupSystems = true;
        }

		public override void Heuristic(in ActionBuffers actionsOut)
        {
	        var actionB = actionsOut.DiscreteActions;
	        actionB[0] = NumberKey;
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
            if (_bForcedAction)
            {
	            _bForcedAction = false;
	            action = _forcedAction;
            }
            
            var system = adventurerInput.GetEconomySystem(this);
            system.AgentSetChoice(this, action);
        }

        private void SetAction(EAdventurerAgentChoices choice)
        {
	        _bForcedAction = true;
	        _forcedAction = choice;
        }

        public void SetAction(int input)
        {
	        var action = (EAdventurerAgentChoices) input;
	        
	        SetAction(action);
        }
        
        public int maxLevel = 5;

        public void LevelUpCheck(int level)
        {
	        if (level == maxLevel)
	        {
		        EndEpisode();
	        }
        }
    }
}