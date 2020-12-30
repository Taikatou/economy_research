using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    public enum EShopScreen {Main = 0, Request = 1, Craft = 2}
    public class ShopAgent : AgentScreen<EShopScreen>
    {
        public ShopInput shopInput;
        public EconomyWallet wallet;
        public CraftingInventory craftingInventory;
        public AgentInventory agentInventory;

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

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = NumberKey;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            var action = Mathf.FloorToInt(vectorAction[0]);
            shopInput.SetAction(this, action);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Player Observations
            sensor.AddObservation((int)ChosenScreen);
            sensor.AddObservation(wallet ? (float)wallet.Money : 0.0f);

            foreach (var sense in shopInput.GetSenses(this))
            {
                sensor.AddObservation(sense);
            }
        }
    }
}
