using System.Collections.Generic;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    public class AgentInventory : LastUpdate
    {
        public List<InventoryItem> Items { get; private set; }

        public List<InventoryItem> startInventory;

        public int ItemCount => Items.Count;

        private void Start()
        {
            ResetInventory();
        }

        public void AddItem(InventoryItem item)
        {
            Items.Add(item);
            Refresh();
        }

        public void ResetInventory()
        {
            if (Items == null)
            {
                Items = new List<InventoryItem>();
            }
            else
            {
                Items.Clear();
            }

            foreach (var item in startInventory)
            {
                var generatedItem = ScriptableObject.CreateInstance("InventoryItem") as InventoryItem;
                if (generatedItem != null)
                {
                    generatedItem.Init(item);

                    Items.Add(generatedItem);
                }
            }

            Refresh();
        }

        public void DecreaseDurability(InventoryItem item)
        {
            if (Items.Contains(item))
            {
                item.DecreaseDurability();
                if (item.Broken)
                {
                    Items.Remove(item);
                }
            }
        }

        public bool ContainsItem(InventoryItem searchItem)
        {
            var found = false;
            foreach (var item in Items)
            {
                if (item.itemName == searchItem.itemName)
                {
                    found = true;
                }
            }

            return found;
        }
    }
}
