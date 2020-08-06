using System.Collections.Generic;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    public struct InventoryItem
    {
        public UsableItem Item;
        public int Number;
    }
    public class AgentInventory : LastUpdate
    {
        public List<UsableItem> startInventory;
        public Dictionary<string, InventoryItem> Items { get; private set; }
        
        private void Start()
        {
            ResetInventory();
        }

        public void AddItem(UsableItem usableItem, int addNumber=1)
        {
            if (!Items.ContainsKey(usableItem.itemName))
            {
                Items.Add(usableItem.itemName, new InventoryItem{Item = usableItem, Number = addNumber});
            }
            else
            {
                var number = Items[usableItem.itemName].Number + addNumber;
                Items[usableItem.itemName] = new InventoryItem{Item = usableItem, Number = number};
            }
            Refresh();
        }

        public void ResetInventory()
        {
            if (Items == null)
            {
                Items = new Dictionary<string, InventoryItem>();
            }
            else
            {
                Items.Clear();
            }

            foreach (var item in startInventory)
            {
                var generatedItem = ScriptableObject.CreateInstance<UsableItem>();
                if (generatedItem != null)
                {
                    generatedItem.Init(item);

                    AddItem(generatedItem);
                }
            }

            Refresh();
        }

        public void DecreaseDurability(UsableItem item)
        {
            if (Items.ContainsKey(item.itemName))
            {
                item.DecreaseDurability();
                if (item.Broken)
                {
                    RemoveItem(item);
                }
            }
        }

        public void RemoveItem(UsableItem usableItem, int number=1)
        {
            var item = Items[usableItem.itemName];
            item.Number -= number;
            if (item.Number <= 0)
            {
                Items.Remove(usableItem.itemName);
            }
            Refresh();
        }

        public bool ContainsItem(UsableItem searchItem)
        {
            var found = false;
            foreach (var item in Items)
            {
                if (item.Key == searchItem.itemName)
                {
                    found = true;
                }
            }

            return found;
        }
    }
}
