using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Inventory;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    // Main and Shop is not used by agent
    public enum AgentScreen { Main, Quest, Auction, Request }

    public enum AuctionChoice { Ignore, Bid }
    
    public class AdventurerAgent : AgentScreen<AgentScreen>
    {
        public AgentInventory inventory;

        public AdventurerInventory adventurerInventory;

        public EconomyWallet wallet;

        public GameAuction gameAuction;

        public PlayerInput playerInput;

        public override AgentScreen ChosenScreen => playerInput.GetScreen(this, AgentScreen.Main);

        public override void OnEpisodeBegin()
        {
            var reset = GetComponentInParent<ResetScript>();
            reset.Reset();
        }

        public void ResetEconomyAgent()
        {
            inventory.ResetInventory();
            wallet.Reset();
        }

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = NumberKey;

            // WUCC check for errors. 
        }

        public void BoughtItem(InventoryItem item, float cost)
        {
            inventory.AddItem(item);
            wallet.SpendMoney(cost);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            var action = Mathf.FloorToInt(vectorAction[0]);
            playerInput.SetAgentAction(this, action);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Player Observations
            sensor.AddObservation((int)ChosenScreen);
            sensor.AddObservation(wallet ? (float)wallet.Money : 0.0f);
            sensor.AddObservation(adventurerInventory.EquipedItem);
            
            // Player Input Observations
            sensor.AddObservation(playerInput.GetProgress(this));
            //sensor.AddObservation(playerInput.gameAuction.getnu);
            //sensor.AddObservation(playerInput.GetNumberInQuest());
            
            // Auction Observations
            var highestBidder = gameAuction.IsHighestBidder(this);
            
            sensor.AddObservation(gameAuction.auctionedItem);
            sensor.AddObservation(highestBidder);
            sensor.AddObservation(gameAuction.currentItemPrice);
            sensor.AddObservation(gameAuction.BidLast);
            sensor.AddObservation(gameAuction.BidOn);
        }
    }
}