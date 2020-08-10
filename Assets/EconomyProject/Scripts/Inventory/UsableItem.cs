﻿using System;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    [Serializable]
    public struct UsableItemDetails
    {
        public string itemName;
        
        public int baseDurability;
        
        public float efficiency;

        public UsableItemDetails(UsableItemDetails itemDetails)
        {
            itemName = itemDetails.itemName;
            baseDurability = itemDetails.baseDurability;
            efficiency = itemDetails.efficiency;
        }

        public int numLootSpawns => 0;
    }
    
    [CreateAssetMenu]
    public class UsableItem : ScriptableObject
    {
        [HideInInspector]
        public int durability;

        public bool unBreakable;

        public UsableItemDetails itemDetails;
        public bool Broken => !unBreakable && durability <= 0;

        private void OnEnable()
        {
            durability = itemDetails.baseDurability;
        }

        public void Init(UsableItem item)
        {
            unBreakable = item.unBreakable;
            durability = item.durability;
            itemDetails = new UsableItemDetails(item.itemDetails);
        }

        public void DecreaseDurability()
        {
            if(!unBreakable)
            {
                durability--;
            }
        }

        public static UsableItem GenerateItem(UsableItem selectedItem)
        {
            var generatedItem = CreateInstance<UsableItem>();
            generatedItem.Init(selectedItem);
            return generatedItem;
        }

        public override string ToString()
        {
            return itemDetails.itemName;
        }
    }
}