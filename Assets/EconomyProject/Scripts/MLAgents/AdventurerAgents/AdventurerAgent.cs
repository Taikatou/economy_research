using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy.Systems;
using Inventory;
using LevelSystem;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public class AdventurerAgent : AgentScreen<EAdventurerScreen>, IEconomyAgent
    {
	    public EAdventurerTypes adventurerType;
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;
        public AdventurerFighterData fighterData;
        public LevelUpComponent levelUpComponent;

		public override AgentType agentType => AgentType.Adventurer;

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

		private EAdventurerAgentChoices _forcedAction;
		private bool _bForcedAction;
		private IEconomyAgent _economyAgentImplementation;

		public void Start()
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

        public void SetAction(EAdventurerAgentChoices choice)
        {
	        _bForcedAction = true;
	        _forcedAction = choice;
        }

        public void SetAction(int input)
        {
	        var action = (EAdventurerAgentChoices) input;
	        Debug.Log(action);
	        
	        SetAction(action);
        }
    }
}