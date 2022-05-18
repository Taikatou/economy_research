using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	public enum ECraftingChoice { BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, MasterSword, UltimateSwordOfPower }
	public enum EAdventurerTypes { All, Tank, Healer, Brawler }

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
	    public ECraftingChoice craftChoice;
	    public List<EAdventurerTypes> validAdventurer = new List<EAdventurerTypes> { EAdventurerTypes.All };
		public UsableItemDetails itemDetails;
		public Guid UniqueId { get; private set; }

        public void Init(UsableItem item)
        {
            UniqueId = Guid.NewGuid();
            itemDetails = new UsableItemDetails(item.itemDetails);

            var containsDamages = ItemData.BaseDamages.ContainsKey(itemDetails.itemName);
            var containsDurabilities = ItemData.BaseDurabilities.ContainsKey(itemDetails.itemName);
			//To use custom parameters
			if(containsDamages && containsDurabilities)
			{
				itemDetails.damage = ItemData.BaseDamages[itemDetails.itemName];
				itemDetails.baseDurability = ItemData.BaseDurabilities[itemDetails.itemName];
				itemDetails.durability = ItemData.BaseDurabilities[itemDetails.itemName];
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
