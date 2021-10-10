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
        public bool canObtainAllWeapons;
        public AdventurerAgent agent;
        
        public AgentInventory agentInventory;
        private Dictionary<string, List<UsableItem>> Items => agentInventory.Items;
        public UsableItem EquipedItem
        {
            get
            {
                UsableItem toReturn = null;
                if (Items != null)
                {
                    if (Items.Count > 0)
                    {
                        var validItems = Items.Where(i => i.Value[0].validAdventurer.Contains(agent.adventurerType));
                        if (validItems.Any())
                        {
                            var max = validItems.Max(x => x.Value[0].itemDetails.damage);   
                            var maxWeapon =
                                Items.First(x => Math.Abs(x.Value[0].itemDetails.damage - max) < 0.01);
                            toReturn =  maxWeapon.Value[0];
                        }
                    }
                }
                return toReturn;
            }
        }

        public void Start()
        {
            agentInventory.checkIfAdd = CanObtainItem;
        }

        public bool CanObtainItem(UsableItem usableItem)
        {
            var valid = canObtainAllWeapons;
            if (!canObtainAllWeapons)
            {
                var filter = new[] { agent.adventurerType, EAdventurerTypes.All };
                valid = usableItem.validAdventurer.Intersect(filter).Any();
            }

            return valid;
        }

        public void DecreaseDurability()
        {
            agentInventory.DecreaseDurability(EquipedItem);
        }
    }
}
