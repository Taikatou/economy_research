using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    [RequireComponent(typeof(AgentInventory))]
    public class AdventurerInventory : MonoBehaviour
    {
        public AgentInventory agentInventory;
        private Dictionary<string, List<UsableItem>> Items => agentInventory.Items;
        public UsableItem EquipedItem
        {
            get
            {
                if (Items != null)
                {
                    if (Items.Count > 0)
                    {
                        var max = Items.Max(x => x.Value[0].itemDetails.damage);
                        var maxWeapon = Items.First(x => Math.Abs(x.Value[0].itemDetails.damage - max) < 0.01);
                        return maxWeapon.Value[0];
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
