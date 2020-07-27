using System.Linq;

namespace EconomyProject.Scripts.Inventory.LootBoxes.Generated
{
    [System.Serializable]
    public class GenericLootDropTableGameObject : GenericLootDropTable<GeneratedLootItemScriptableObject, InventoryItem>
    {
        public float MaxMoney => lootDropItems.Max(x => x.item.baseBidPrice);
    }
}
