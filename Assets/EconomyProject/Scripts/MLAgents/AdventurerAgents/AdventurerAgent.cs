using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using Inventory;
using Unity.MLAgents.Sensors;
using UnityEngine;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
	public enum EAdventurerAgentChoices { None = 0, MainMenu, ResourceRequest, Shop, Adventure, TakeResourceRequest, PurchaseItem, AdventureForest, AdventureSea, AdventureMountain, AdventureVolcano, BattleAttack, BattleHeal, BattleFlee }

	//main and shop is not used by agent
	public enum EAdventurerScreen { Main=0, Request=1, Shop=2, Adventurer=3, Rest }
    
    public class AdventurerAgent : AgentScreen<EAdventurerScreen>
    {
		public EAdventurerAgentChoices agentChoice;
		public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker;

		public override AgentType agentType { get { return AgentType.Adventurer; } }
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

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = NumberKey;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            var action = Mathf.FloorToInt(vectorAction[0]);
            adventurerInput.SetAgentAction(this, action);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Player Observations
            sensor.AddObservation((int)ChosenScreen);
            sensor.AddObservation(wallet ? wallet.Money : 0.0f);
            sensor.AddObservation(adventurerInventory.EquipedItem.itemDetails.damage);
            
            var requests = requestTaker.requestSystem.craftingRequestRecord.GetCurrentRequests(requestTaker);
            foreach (var request in requests)
            {
                sensor.AddObservation((int)request.Resource);
                sensor.AddObservation(request.Number);
                
                var amount = requestTaker.GetCurrentStock(request.Resource);
                sensor.AddObservation(amount);
            }

            foreach (var sense in requestTaker.GetSenses())
            {
                sensor.AddObservation(sense);
            }

            foreach (var sense in adventurerInput.GetSenses(this))
            {
                sensor.AddObservation(sense);
            }
        }

		/// <summary>
		/// Manage UI 
		/// </summary>
		/// <param name="choice">Int associate to a specific UI action</param>
		/// <param name="resourceRequestToTake">Mandatory if choice = TakeResourceRequest</param>
		/// <param name="itemToBuy">Mandatory if choice = PurchaseItem</param>
		public void SetAction(EAdventurerAgentChoices choice, CraftingResourceRequest resourceRequestToTake = null, UsableItem itemToBuy = null)
		{
			agentChoice = choice;
			switch (choice)
			{
				case EAdventurerAgentChoices.None:
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
				case EAdventurerAgentChoices.TakeResourceRequest:
					requestTaker.TakeRequest(resourceRequestToTake);
					break;
				case EAdventurerAgentChoices.PurchaseItem:
					AgentShopSubSystem shopCraftingSystem = GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>().system.shopSubSubSystem;
					ShopAgent shopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>().CurrentAgent;

					shopCraftingSystem.PurchaseItem(shopAgent, itemToBuy.itemDetails, this.wallet, this.adventurerInventory.agentInventory);
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
				default:
					Debug.Log("Wrong EAdventurerAgentChoices : " + choice);
					break;
			}
		}
	}
}