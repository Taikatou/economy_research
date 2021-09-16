using System;
using System.Collections;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using UnityEngine;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.UI.Inventory;
using Inventory;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	//main and shop is not used by agent

	public class AdventurerAgent : AgentScreen<EAdventurerScreen>, IEconomyAgent
    {
	    public EAdventurerTypes adventurerType;
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;

		public override AgentType agentType => AgentType.Adventurer;
		public override EAdventurerScreen ChosenScreen => adventurerInput.GetScreen(this, EAdventurerScreen.Main);

		private EAdventurerAgentChoices _forcedAction;
		private bool _bForcedAction;
		private IEconomyAgent _economyAgentImplementation;

		public override void OnEpisodeBegin()
        {
			var reset = GetComponentInParent<ResetScript>();
			if (reset != null)
			{
				reset.Reset();
			}
        }

        public void ResetEconomyAgent()
        {
            inventory.ResetInventory();
            wallet.Reset();
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
            system.SetChoice(this, action);
        }

        public void Update()
        {
	        
        }

        public void SetAction(EAdventurerAgentChoices choice)
        {
	        _bForcedAction = true;
	        _forcedAction = choice;
        }

        /// <summary>
		/// Manage UI 
		/// </summary>
		/// <param name="choice">Int associate to a specific UI action</param>
		/// <param name="resourceRequestToTake">Mandatory if choice = TakeResourceRequest</param>
		/// <param name="itemToBuy">Mandatory if choice = PurchaseItem</param>
		public void SetAction(EAdventurerAgentChoices choice, CraftingResourceRequest resourceRequestToTake, ShopItem? itemToBuy = null)
		{
			switch (choice)
			{
				case EAdventurerAgentChoices.None:
					break;
				case EAdventurerAgentChoices.TakeRequest:
					requestTaker.TakeRequest(resourceRequestToTake);
					break;
				case EAdventurerAgentChoices.MainMenu:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Main);
					break;
				case EAdventurerAgentChoices.FindRequest:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Request);
					break;
				case EAdventurerAgentChoices.Shop:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Shop);
					break;
				case EAdventurerAgentChoices.Adventure:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Adventurer);
					break;
				case EAdventurerAgentChoices.AForest:
					AdventurerInput.AdventurerSystem.system.StartBattle(this, BattleEnvironments.Forest);
					break;
				case EAdventurerAgentChoices.ASea:
					AdventurerInput.AdventurerSystem.system.StartBattle(this, BattleEnvironments.Sea);
					break;
				case EAdventurerAgentChoices.AMountain:
					AdventurerInput.AdventurerSystem.system.StartBattle(this, BattleEnvironments.Mountain);
					break;
				case EAdventurerAgentChoices.AVolcano:
					AdventurerInput.AdventurerSystem.system.StartBattle(this, BattleEnvironments.Volcano);
					break;
				case EAdventurerAgentChoices.BAttack:
					AdventurerInput.AdventurerSystem.system.OnAttackButton(this);
					break;
				case EAdventurerAgentChoices.BHeal:
					AdventurerInput.AdventurerSystem.system.OnHealButton(this);
					break;
				case EAdventurerAgentChoices.BFlee:
					AdventurerInput.AdventurerSystem.system.OnFleeButton(this);
					break;
				case EAdventurerAgentChoices.PurchaseItem:
					var shopCrafting = FindObjectOfType<ShopCraftingSystemBehaviour>();
					if (shopCrafting)
					{
						var shopCraftingSystem = shopCrafting.system.shopSubSubSystem;
						if (itemToBuy != null && shopCraftingSystem != null)
						{
							shopCraftingSystem.PurchaseItem(itemToBuy.Value.Seller,
															itemToBuy.Value.Item.itemDetails,
															wallet, 
															adventurerInventory.agentInventory);	
						}	
					}
					break;
				default:
					Debug.Log("Wrong EAdventurerAgentChoices : " + choice);
					break;
			}
		}

        public void SetAction(int input)
        {
	        var action = (EAdventurerAgentChoices) input;
	        Debug.Log(action);
	        
	        SetAction(action);
        }
    }
}