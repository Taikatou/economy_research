using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    [RequireComponent(typeof(AgentInventory))]
    public class AdventurerInventory : MonoBehaviour
    {
        public AgentInventory agentInventory;
        private List<InventoryItem> Items => agentInventory.Items;

        public InventoryItem EquipedItem
        {
            get
            {
                if (Items != null)
                {
                    if (Items.Count > 0)
                    {
                        var max = Items.Max(x => x.efficiency);
                        var maxWeapon = Items.First(x => Math.Abs(x.efficiency - max) < 0.01);
                        return maxWeapon;
                    }
                    else
                    {
                        Debug.Log("Items = 0");
                    }
                }
                else
                {
                    Debug.Log("Null");
                }
                return null;
            }
        }

        public void DecreaseDurability()
        {
            agentInventory.DecreaseDurability(EquipedItem);
        }
    }
}
