﻿using EconomyProject.Scripts.GameEconomy;
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
        public EconomyWallet wallet;
        public CraftingInventory craftingInventory;
        public AgentInventory agentInventory;

        public override EShopScreen ChosenScreen
        {
            get
            {
                var screen = shopInput.GetScreen(this, EShopScreen.Main);
                Debug.Log(screen);
                return screen;
            }
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
            sensor.AddObservation((float)wallet.EarnedMoney);
            sensor.AddObservation((float)wallet.SpentMoney);
            sensor.AddObservation((float)wallet.Money);
            wallet.ResetStep();
        }
    }
}
