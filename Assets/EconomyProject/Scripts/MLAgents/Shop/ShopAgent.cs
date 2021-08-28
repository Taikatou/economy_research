using EconomyProject.Scripts.GameEconomy;
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

		public override AgentType agentType { get { return AgentType.Shop; } }
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
            var action = Mathf.FloorToInt(actions.DiscreteActions[0]);
            shopInput.SetAction(this, action);
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
		public void SetAction(EShopAgentChoices choice, CraftingResourceUi resourceRequest = null, CraftingInfo craftingChoice = null, UsableItem item = null)
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
					shopInput.requestSystem.system.MakeChoice(this, resourceRequest.ResourceType);
					break;
				case EShopAgentChoices.CraftItem:
					shopInput.shopCraftingSystem.system.craftingSubSubSystem.MakeRequest(this, (int)craftingChoice.craftingMap.choice);
					break;
				case EShopAgentChoices.SubmitToShop:
					shopInput.shopCraftingSystem.system.shopSubSubSystem.SubmitToShop(this, item);
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
