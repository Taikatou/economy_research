using System;
using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    [Serializable]
    public struct UsableItemDetails
    {
        public bool unBreakable;
        
        public string itemName;
        
        public int baseDurability;
        
        public int damage;
        
        [HideInInspector]
        public int durability;
        
        public bool Broken => !unBreakable && durability <= 0;

        public UsableItemDetails(UsableItemDetails itemDetails)
        {
            itemName = itemDetails.itemName;
            baseDurability = itemDetails.baseDurability;
            damage = itemDetails.damage;
            durability = itemDetails.baseDurability;
            baseDurability = itemDetails.durability;
            unBreakable = itemDetails.unBreakable;
        }
        
        public void DecreaseDurability()
        {
            if(!unBreakable)
            {
                durability--;
            }
        }

        public void ResetDurability()
        {
            durability = baseDurability;
        }
    }
    
    [CreateAssetMenu]
    public class UsableItem : ScriptableObject
    {
        public UsableItemDetails itemDetails;

        public Guid UniqueId { get; private set; }

        public void Init(UsableItem item)
        {
            UniqueId = Guid.NewGuid();
            itemDetails = new UsableItemDetails(item.itemDetails);
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
