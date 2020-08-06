using UnityEngine;

namespace EconomyProject.Scripts.Inventory
{
    [CreateAssetMenu]
    public class UsableItem : ScriptableObject
    {
        public string itemName;
        
        public int baseDurability;

        [HideInInspector]
        public int durability;

        public bool unBreakable;

        public bool Broken => !unBreakable && durability <= 0;

        public float efficiency;

        public int numLootSpawns = 1;

        private void OnEnable()
        {
            durability = baseDurability;
        }

        public void Init(UsableItem item)
        {
            itemName = item.itemName;
            baseDurability = item.baseDurability;
            efficiency = item.efficiency;
            unBreakable = item.unBreakable;
            durability = item.durability;
            numLootSpawns = item.numLootSpawns;
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
    }
}
