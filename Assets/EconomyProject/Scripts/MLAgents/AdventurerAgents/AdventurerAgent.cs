using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Inventory;
using TurnBased.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    // Main and Shop is not used by agent
    public enum EAdventurerScreen { Main=0, Request=1, Shop=2, Adventurer=3 }
    
    public class AdventurerAgent : AgentScreen<EAdventurerScreen>
    {
        public AgentInventory inventory;
        public AdventurerInventory adventurerInventory;
        public EconomyWallet wallet;
        public AdventurerInput adventurerInput;
        public AdventurerRequestTaker requestTaker; 

        public override EAdventurerScreen ChosenScreen => adventurerInput.GetScreen(this, EAdventurerScreen.Main);

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
            sensor.AddObservation(wallet ? (float)wallet.Money : 0.0f);
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
    }
}