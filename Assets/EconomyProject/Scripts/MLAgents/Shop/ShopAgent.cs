using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.Shop
{
	public enum EShopAgentChoices { None = 0, Back, SubmitToShop, IncreasePrice, DecreasePrice, Up, Down, Select, MakeRequest, ChangeRequest, EditPrice, RemoveRequest }

	public enum EShopScreen { Main = EShopAgentChoices.None, Request, Craft}
    
	public class ShopAgent : AgentScreen<EShopScreen>, IEconomyAgent
    {
	    public ShopInput shopInput;
        public EconomyWallet wallet;
        public CraftingInventory craftingInventory;
        public AgentInventory agentInventory;

        private EShopAgentChoices _forcedAction;
        private bool _bForcedAction;
		public override AgentType agentType => AgentType.Shop;

		public void Start()
		{
			wallet.onEarnMoney = OnEarnMoney;
			agentInventory.onItemAdd += OnCraft;
		}

		private void OnEarnMoney(float amount)
		{
			if (TrainingConfig.OnSell)
			{
				AddReward(TrainingConfig.OnSellReward);
			}
		}

		private void OnCraft(UsableItem usableItem)
		{
			if (TrainingConfig.OnCraft)
			{
				AddReward(TrainingConfig.OnCraftReward);
			}
		}

		public override EShopScreen ChosenScreen
        {
            get
            {
	            var toReturn = EShopScreen.Main;
	            if (shopInput != null)
	            {
		            toReturn = shopInput.GetScreen(this, EShopScreen.Main);
	            }
                
                return toReturn;
            }
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            
            agentInventory.Setup();
            craftingInventory.ResetInventory();
            agentInventory.Setup();
            wallet.Setup(shopInput.requestSystem.system.requestSystem, AgentType.Adventurer);
            
            ResetOnItem.bSetupSystems = true;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
	        var actionB = actionsOut.DiscreteActions;
	        actionB[0] = NumberKey;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
	        var action = (EShopAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);;
	        if (_bForcedAction)
	        {
		        _bForcedAction = false;
		        action = _forcedAction;
	        }
	        
	        var system = shopInput.GetEconomySystem(this);
            system.AgentSetChoice(this, action);
        }
        
		/// <summary>
		/// Manage UI 
		/// </summary>
		/// <param name="choice">Int associate to a specific UI action</param>
		/// <param name="resourceRequest">Mandatory if choice = MakeResourceRequest</param>
		/// <param name="craftingChoice">Mandatory if choice = CraftItem</param>
		/// <param name="item">Mandatory if choice = SubmitToShop or choice = IncreasePrice or choice = DecreasePrice</param>
		public void SetAction(EShopAgentChoices choice, ECraftingResources? resourceRequest = null, ECraftingChoice? craftingChoice = null, UsableItem item = null)
		{
			switch (choice)
			{
				case EShopAgentChoices.None:
					shopInput.ChangeScreen(this, EShopScreen.Main);
					break;
				case EShopAgentChoices.SubmitToShop:
					if (item)
					{
						shopInput.shopCraftingSystem.system.shopSubSubSystem.SubmitToShop(this, item);	
					}
					break;
				case EShopAgentChoices.IncreasePrice:
					shopInput.shopCraftingSystem.system.SetPrice(this, item, 1);
					break;
				case EShopAgentChoices.DecreasePrice:
					shopInput.shopCraftingSystem.system.SetPrice(this, item, -1);
					break;
			}
		}

		public void SetAction(EShopAgentChoices choice)
		{
			_bForcedAction = true;
			_forcedAction = choice;
		}

		public void SetAction(int action)
		{
			SetAction((EShopAgentChoices) action);
		}

		public IEnumerable<EnabledInput> GetEnabledInput()
		{
			return shopInput.GetActionMask(this);
		}
    }
}
