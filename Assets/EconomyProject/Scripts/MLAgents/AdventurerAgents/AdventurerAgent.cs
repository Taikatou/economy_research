using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using Unity.MLAgents.Sensors;
using UnityEngine;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.UI.Inventory;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public enum EAdventurerAgentChoices { None = 0, MainMenu, ResourceRequest, Shop, Adventure, TakeResourceRequest, PurchaseItem, AdventureForest, AdventureSea, AdventureMountain, AdventureVolcano, BattleAttack, BattleHeal, BattleFlee }

	//main and shop is not used by agent
	public enum EAdventurerScreen { Main=0, Request=1, Shop=2, Adventurer=3, Rest=4 }
    
    public class AdventurerAgent : AgentScreen<EAdventurerScreen>
    {
		public EAdventurerAgentChoices agentChoice;
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;

		public override AgentType agentType => AgentType.Adventurer;
		public override EAdventurerScreen ChosenScreen => adventurerInput.GetScreen(this, EAdventurerScreen.Main);

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

        public override void OnActionReceived(ActionBuffers actions)
        {
            var action = Mathf.FloorToInt(actions.DiscreteActions[0]);
            adventurerInput.SetAgentAction(this, action);
        }

        /// <summary>
		/// Manage UI 
		/// </summary>
		/// <param name="choice">Int associate to a specific UI action</param>
		/// <param name="resourceRequestToTake">Mandatory if choice = TakeResourceRequest</param>
		/// <param name="itemToBuy">Mandatory if choice = PurchaseItem</param>
		public void SetAction(EAdventurerAgentChoices choice, CraftingResourceRequest resourceRequestToTake = null, ShopItemUi? itemToBuy = null)
		{
			agentChoice = choice;
			switch (choice)
			{
				case EAdventurerAgentChoices.None:
					break;
				case EAdventurerAgentChoices.TakeResourceRequest:
					requestTaker.TakeRequest(resourceRequestToTake);
					break;
				case EAdventurerAgentChoices.MainMenu:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Main);
					break;
				case EAdventurerAgentChoices.ResourceRequest:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Request);
					break;
				case EAdventurerAgentChoices.Shop:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Shop);
					break;
				case EAdventurerAgentChoices.Adventure:
					adventurerInput.ChangeScreen(this, EAdventurerScreen.Adventurer);
					break;
				case EAdventurerAgentChoices.AdventureForest:
					adventurerInput.adventurerSystem.system.StartBattle(this, BattleEnvironments.Forest);
					break;
				case EAdventurerAgentChoices.AdventureSea:
					adventurerInput.adventurerSystem.system.StartBattle(this, BattleEnvironments.Sea);
					break;
				case EAdventurerAgentChoices.AdventureMountain:
					adventurerInput.adventurerSystem.system.StartBattle(this, BattleEnvironments.Mountain);
					break;
				case EAdventurerAgentChoices.AdventureVolcano:
					adventurerInput.adventurerSystem.system.StartBattle(this, BattleEnvironments.Volcano);
					break;
				case EAdventurerAgentChoices.BattleAttack:
					adventurerInput.adventurerSystem.system.OnAttackButton(this);
					break;
				case EAdventurerAgentChoices.BattleHeal:
					adventurerInput.adventurerSystem.system.OnHealButton(this);
					break;
				case EAdventurerAgentChoices.BattleFlee:
					adventurerInput.adventurerSystem.system.OnFleeButton(this);
					break;
				case EAdventurerAgentChoices.PurchaseItem:
					var shopCraftingSystem = FindObjectOfType<ShopCraftingSystemBehaviour>().system.shopSubSubSystem;
					if (itemToBuy != null && shopCraftingSystem != null)
					{
						shopCraftingSystem.PurchaseItem(itemToBuy.Value.Seller, itemToBuy.Value.Item.itemDetails,
							wallet, adventurerInventory.agentInventory);	
					}
					break;
				default:
					Debug.Log("Wrong EAdventurerAgentChoices : " + choice);
					break;
			}
		}
    }
}