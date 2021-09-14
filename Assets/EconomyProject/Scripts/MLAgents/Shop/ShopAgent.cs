using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using Unity.MLAgents.Sensors;
using UnityEngine;
using EconomyProject.Scripts.UI.Craftsman.Request.ScrollList;
using EconomyProject.Scripts.UI.Craftsman.Crafting;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.Shop
{
	public enum EShopAgentChoices { None = 0, MainMenu, RequestResource, Craft, MakeResourceRequest, CraftItem, SubmitToShop, IncreasePrice, DecreasePrice }

	public enum EShopScreen {Main = 0, Request = 1, Craft = 2}
    public class ShopAgent : AgentScreen<EShopScreen>
    {
		public EShopAgentChoices agentChoice;
		public ShopInput shopInput;
        public EconomyWallet wallet;
        public CraftingInventory craftingInventory;
        public AgentInventory agentInventory;

		public override AgentType agentType => AgentType.Shop;
		
		private EShopAgentChoices _forcedAction;
		private bool _bForcedAction;

		public override EShopScreen ChosenScreen
        {
            get
            {
                var screen = shopInput.GetScreen(this, EShopScreen.Main);
                return screen;
            }
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            agentInventory.ResetInventory();
            craftingInventory.ResetInventory();
            wallet.Reset();
        }

        public void ResetEconomyAgent()
        {
            EndEpisode();
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
            system.SetChoice(this, action);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Player Observations
            sensor.AddObservation((int)ChosenScreen);
            sensor.AddObservation(wallet ? (float)wallet.Money : 0.0f);

            foreach (var sense in shopInput.GetObservations(this))
            {
                sensor.AddObservation(sense);
            }
        }

		/// <summary>
		/// Manage UI 
		/// </summary>
		/// <param name="choice">Int associate to a specific UI action</param>
		/// <param name="resourceRequest">Mandatory if choice = MakeResourceRequest</param>
		/// <param name="craftingChoice">Mandatory if choice = CraftItem</param>
		/// <param name="item">Mandatory if choice = SubmitToShop or choice = IncreasePrice or choice = DecreasePrice</param>
		public void SetAction(EShopAgentChoices choice, CraftingResources? resourceRequest = null, CraftingChoice? craftingChoice = null, UsableItem item = null)
		{
			agentChoice = choice;
			switch (choice)
			{
				case EShopAgentChoices.None:
					break;
				case EShopAgentChoices.MainMenu:
					shopInput.ChangeScreen(this, EShopScreen.Main);
					break;
				case EShopAgentChoices.RequestResource:
					shopInput.ChangeScreen(this, EShopScreen.Request);
					break;
				case EShopAgentChoices.Craft:
					shopInput.ChangeScreen(this, EShopScreen.Craft);
					break;
				case EShopAgentChoices.MakeResourceRequest:
					if (resourceRequest.HasValue)
					{
						shopInput.requestSystem.system.MakeChoice(this, resourceRequest.Value);
					}
					break;
				case EShopAgentChoices.CraftItem:
					if (craftingChoice.HasValue)
					{
						shopInput.shopCraftingSystem.system.craftingSubSubSystem.MakeRequest(this, craftingChoice.Value);
					}
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
				default:
					Debug.Log("Wrong EShopAgentChoices : " + choice);
					break;
			}
		}
	}
}
