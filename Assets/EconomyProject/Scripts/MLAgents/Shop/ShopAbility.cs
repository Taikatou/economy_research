using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    [System.Serializable]
    public class ShopItem
    {
        public InventoryItem inventoryItem;
        public int price;

        public int stock;
        
        public readonly ShopAgent Seller;

        public ShopItem(ShopItem item, int stock, ShopAgent seller)
        {
            inventoryItem = item.inventoryItem;
            price = item.price;
            this.stock = stock;
            Seller = seller;
        }

        public string ItemName => inventoryItem ? inventoryItem.itemName : "";

        public bool DeductStock(int number)
        {
            var newValue = this.stock - number;
            stock = newValue < 0 ? 0 : newValue;
            return newValue == 0;
        }

        public void IncreaseStock(int number)
        {
            if (number > 0)
            {
                stock += number;
            }
        }

        public static bool Compare(ShopItem itemA, ShopItem itemB)
        {
            return itemA.inventoryItem.itemName == itemB.inventoryItem.itemName &&
                   itemA.price == itemB.price;
        }

        public void IncreasePrice(int increase)
        {
            if (increase > 0)
            {
                price += increase; 
            }
        }

        public void DecreasePrice(int increase)
        {
            if (increase > 0)
            {
                price -= increase;
            }
        }
    }

    public class ShopAbility : MonoBehaviour
    {
        public List<ShopItem> shopItems;

        public void ChangePrice(ShopItem item, int price)
        {
            if (price > 0)
            {
                var shopItem = FindShopItem(item.inventoryItem);
                if (shopItem != null)
                {
                    shopItem.price = price;
                }
            }
        }

        private ShopItem FindShopItem(InventoryItem item)
        {
            foreach (var shopItem in shopItems)
            {
                if (shopItem.ItemName == item.itemName)
                {
                    return shopItem;
                }
            }
            return null;
        }
    }
}
