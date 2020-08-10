using System.Collections.Generic;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    public class AgentInventory : LastUpdate
    {
        public List<UsableItem> startInventory;
        public Dictionary<string, List<UsableItem>> Items { get; private set; }
        
        private void Start()
        {
            ResetInventory();
        }

        public void AddItem(UsableItem usableItem)
        {
            if (!Items.ContainsKey(usableItem.ToString()))
            {
                Items.Add(usableItem.ToString(), new List<UsableItem>());
            }

            Items[usableItem.ToString()].Add(usableItem);
            Refresh();
        }

        public void ResetInventory()
        {
            if (Items == null)
            {
                Items = new Dictionary<string, List<UsableItem>>();
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
            if (Items.ContainsKey(item.ToString()))
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
            var item = Items[usableItem.ToString()];
            Destroy(item[0]);
            if (item.Count <= 0)
            {
                Items.Remove(usableItem.ToString());
            }
            Refresh();
        }

        public bool ContainsItem(UsableItem searchItem)
        {
            var found = false;
            foreach (var item in Items)
            {
                if (item.Key == searchItem.ToString())
                {
                    found = true;
                }
            }

            return found;
        }
    }
}
