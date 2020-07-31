using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    public enum EShopScreen {Main = 0, Request = 1, Craft = 2, MarketPlace = 3}
    public class ShopAgent : AgentScreen<EShopScreen>
    {
        public ShopInput shopInput;
        public List<ShopItem> ItemList => GetComponent<ShopAbility>().shopItems;
        public EconomyWallet Wallet => GetComponent<EconomyWallet>();
        public CraftingInventory CraftingInventory => GetComponent<CraftingInventory>();
        public AgentInventory AgentInventory => GetComponent<AgentInventory>();
        public override EShopScreen ChosenScreen => shopInput.GetScreen(this, EShopScreen.Main);

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = NumberKey;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            var action = Mathf.FloorToInt(vectorAction[0]);
            shopInput.SetAction(this, action);
        }

        public void RemoveItem(ShopItem itemToRemove)
        {
            var sellersItem = FindItem(itemToRemove);
            Debug.Log(sellersItem.stock);
            var toRemove = sellersItem.DeductStock(itemToRemove.stock);
            if (toRemove)
            {
                ItemList.Remove(sellersItem);
            }
        }

        public void AddItem(ShopItem addItem)
        {
            var foundItem = FindItem(addItem);
            if (foundItem != null)
            {
                foundItem.IncreaseStock(addItem.stock);
            }
            else
            {
                ItemList.Add(addItem);
            }
        }

        private ShopItem FindItem(ShopItem toCompare)
        {
            foreach (var item in ItemList)
            {
                if (ShopItem.Compare(item, toCompare))
                {
                    return item;
                }
            }

            return null;
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
