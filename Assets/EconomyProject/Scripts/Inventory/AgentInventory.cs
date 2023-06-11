using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    public delegate void OnItemAdd(UsableItem usableItem);
    public delegate bool CheckIfAdd(UsableItem usableItem);
    public sealed class AgentInventory : LastUpdate, ISetup
    {
        public CheckIfAdd checkIfAdd;
        public List<UsableItem> startInventory;
        public Dictionary<string, List<UsableItem>> Items { get; private set; }

        public int NumItems()
        {
            var num = 0;

            foreach (var item in Items)
            {
                num += item.Value.Count;
            }
            return num;
        }
        
        public OnItemAdd onItemAdd;

        public void AddItem(UsableItem usableItem)
        {
            onItemAdd?.Invoke(usableItem);
            
            var ok = checkIfAdd?.Invoke(usableItem);
            if (ok.HasValue)
            {
                if (!ok.Value)
                {
                    return;
                }
            }
            if (!Items.ContainsKey(usableItem.ToString()))
            {
                Items.Add(usableItem.ToString(), new List<UsableItem>());
            }

            Items[usableItem.ToString()].Add(usableItem);
            Refresh();
        }


        public void Setup()
        {
            if(Items == null)
                Items = new Dictionary<string, List<UsableItem>>();
            else
                Items.Clear();

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
                item.itemDetails.DecreaseDurability();
                if (item.itemDetails.Broken)
                {
                    DestroyItem(item);
                    OverviewVariables.ItemBroke();
                }
            }
        }

        private void DestroyItem(UsableItem item)
        {
            RemoveItem(item);
            Destroy(item);
        }

        public void RemoveItem(UsableItem usableItem)
        {
            var key = usableItem.ToString();
            var usableItems = Items[key];
            usableItems.RemoveAll(x => x.UniqueId == usableItem.UniqueId);

            if (usableItems.Count == 0)
            {
                Debug.Log("Remove key");
                Items.Remove(key);
            }

            Refresh();
        }

        public bool ContainsItem(UsableItem searchItem)
        {
			if (Items == null)
			{
				return false;
			}

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
