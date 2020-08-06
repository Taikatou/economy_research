using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.Inventory;
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
        private EconomyWallet Wallet => GetComponent<EconomyWallet>();
        public CraftingInventory CraftingInventory => GetComponent<CraftingInventory>();
        public override EShopScreen ChosenScreen => shopInput.GetScreen(this, EShopScreen.Main);
        public AgentInventory AgentInventory => GetComponent<AgentInventory>();

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
            sensor.AddObservation((float)Wallet.EarnedMoney);
            sensor.AddObservation((float)Wallet.SpentMoney);
            sensor.AddObservation((float)Wallet.Money);
            Wallet.ResetStep();
        }
    }
}
