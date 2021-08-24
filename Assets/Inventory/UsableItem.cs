using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public struct UsableItemDetails
    {
	    public Sprite icon;
	    
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
			unBreakable = itemDetails.unBreakable;
			baseDurability = itemDetails.baseDurability;
            damage = itemDetails.damage;
            durability = itemDetails.baseDurability;
			icon = itemDetails.icon;
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
			
			//To use custom parameters
			if(ItemData.baseDamages.ContainsKey(itemDetails.itemName) && ItemData.baseDurabilities.ContainsKey(itemDetails.itemName))
			{
				itemDetails.damage = ItemData.baseDamages[itemDetails.itemName];
				itemDetails.baseDurability = ItemData.baseDurabilities[itemDetails.itemName];
				itemDetails.durability = ItemData.baseDurabilities[itemDetails.itemName];
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
